namespace UmbracoVendr.App_Code.DatabaseRepo.DatabaseMigration
{
	public class Triggers
	{
		public string ExistsTri_ProviderPaymentLogs_Insert_When_GetOrder()
		{
			return @"SELECT count(*) FROM sysobjects WHERE  name = 'tri_ProviderPaymentLogs_Insert_When_GetOrder'";
		}
		public string Tri_ProviderPaymentLogs_Insert_When_GetOrder()
		{
			var query = @"Create Trigger [dbo].[tri_ProviderPaymentLogs_Insert_When_GetOrder] on [dbo].[vendrOrderLine]
							after Insert
							As

								declare @providerId int;
								declare @description nvarchar(max);
								declare @amount decimal(19,4);
								declare @productReference nvarchar(max);
								declare @OrderId nvarchar(max);

								select @providerId = (select Top 1 MemberId from ProductsTable where ProductNodeKey = i.productReference),
								@description = 'Captured amount by product and Product reference Id is ' + i.productReference,
								@amount = i.unitPrice,
								@OrderId = i.orderId
								from inserted i
								join vendrOrder vo on vo.id = i.orderId;

								Insert into ProviderPaymentLogs(ProviderId,Description,Amount,LogDate,PaymentStatus,OrderId) 
								values(@providerId,@description,@amount, GETDATE(),'uncaptured',@OrderId);";
			return query.Replace("@", "@@"); ;
		}

		public string ExistsTri_ProviderPaymentLogs_When_DeleteOrder()
		{
			return @"SELECT count(*) FROM sysobjects WHERE  name = 'tri_ProviderPaymentLogs_When_DeleteOrder'";
		}
		public string Tri_ProviderPaymentLogs_When_DeleteOrder()
		{
			var query = @"Create Trigger [dbo].[tri_ProviderPaymentLogs_When_DeleteOrder] on [dbo].[vendrOrderLine]
							after Delete
							As
								declare @OrderId nvarchar(max);
								select @OrderId = i.orderId from deleted i;
								Delete ProviderPaymentLogs where OrderId = @OrderId;";
			return query.Replace("@", "@@");
		}

		public string ExistsTri_ProviderPaymentLogs_Update_When_OrderStatusCaptured()
		{
			return @"SELECT count(*) FROM sysobjects WHERE  name = 'tri_ProviderPaymentLogs_Update_When_OrderStatusCaptured'";
		}
		public string Tri_ProviderPaymentLogs_Update_When_OrderStatusCaptured()
		{
			var query = @"Create Trigger [dbo].[tri_ProviderPaymentLogs_Update_When_OrderStatusCaptured] on [dbo].[vendrOrder]
							after Update
							As
								declare @OrderId nvarchar(max);
								declare @paymentStatus int;
								declare @amount decimal(19,4);
								declare @flagAmount decimal(19,4);

								select @paymentStatus = i.paymentStatus,
								@OrderId = i.id
								from inserted i
								join ProviderPaymentLogs ppl on ppl.OrderId = i.id;



								if(@paymentStatus = 2)
								begin
									Update ProviderPaymentLogs set PaymentStatus = 'captured' where OrderId = @OrderId;
								end
								else if(@paymentStatus = 4)
								begin
									Update ProviderPaymentLogs set PaymentStatus = 'refunded' where OrderId = @OrderId;
								end
								else if(@paymentStatus = 3)
								begin
									Update ProviderPaymentLogs set PaymentStatus = 'cancelled' where OrderId = @OrderId;
								end
								else if(@paymentStatus = 5)
								begin
									Update ProviderPaymentLogs set PaymentStatus = 'pendingExternalSystem' where OrderId = @OrderId;
								end
								else if(@paymentStatus = 200)
								begin
									Update ProviderPaymentLogs set PaymentStatus = 'error' where OrderId = @OrderId;
								end";
			return query.Replace("@", "@@");
		}

		public string ExistsTri_ProviderWallet_AddBalance()
		{
			return @"SELECT count(*) FROM sysobjects WHERE  name = 'tri_ProviderWallet_AddBalance'";
		}
		public string Tri_ProviderWallet_AddBalance()
		{
			var query = @"Create Trigger [dbo].[tri_ProviderWallet_AddBalance] on [dbo].[ProviderPaymentLogs]
						after update
						As
							declare @providerId int;
							declare @providerBalance decimal(19,4)
							declare @currentBalance decimal(19,4)
							declare @currentWithdrawalPending decimal(19,4)
							declare @currentTotalPaid decimal(19,4)
							declare @id int;
							declare @paymentStatus nvarchar(max)
							declare @WithdrawalPending decimal(19,4)
							declare @TotalPaid decimal(19,4)
							declare @amount decimal(19,4)
							declare @orderId nvarchar(max)
							declare @paymentStatusExists nvarchar(max)
							declare @AmountFlag nvarchar(max)


							select @providerId = i.ProviderId, @amount = i.Amount,@paymentStatus = i.PaymentStatus, @orderId = i.OrderId, @AmountFlag = i.AmountFlag
							from inserted i;

							select @currentBalance = ProviderBalance, @id= ProviderId,
							@currentWithdrawalPending = WithdrawalPending,
							@currentTotalPaid = (case when TotalPaid is null then 0 else TotalPaid end) 
							from ProviderWallet where ProviderId = @providerId;
	
							select @paymentStatusExists = PaymentStatus from ProviderPaymentLogs where OrderId = @orderId;
	
							if(@paymentStatus = 'captured' and  @AmountFlag is null)
							begin
							
							update pw set ProviderBalance=(h.Amount + ProviderBalance)
							from ProviderWallet pw
							cross join (select pp.ProviderId, sum(pp.Amount) as Amount
							from ProviderPaymentLogs pp where pp.OrderId=@orderId group by pp.ProviderId) h
							where h.ProviderId = pw.ProviderId

							update ProviderPaymentLogs set AmountFlag = 'captured' where OrderId = @orderId
							--set @providerBalance = @amount + @currentBalance;
							--Update ProviderWallet set  ProviderBalance = @providerBalance where ProviderId = @id;
							end
							--else if(@paymentStatus = 'captured')
							--begin
							--	--set @providerBalance = @amount;

							--	Insert into ProviderWallet(ProviderId,ProviderBalance,LastUpdatedDate) 
							--	select pp.ProviderId, sum(pp.Amount), GETDATE() as Amount from ProviderPaymentLogs pp where pp.OrderId=@orderId group by pp.ProviderId 
							--	--Insert into ProviderWallet(ProviderId,ProviderBalance,LastUpdatedDate) 
							--	--values(@providerId, @providerBalance , GETDATE())
							--end
							else if(@paymentStatus = 'approved')
							begin
								set @WithdrawalPending = @currentWithdrawalPending - @amount;
								set @TotalPaid = @currentTotalPaid + @amount;
								Update ProviderWallet
								set  WithdrawalPending = @WithdrawalPending,
								TotalPaid = @TotalPaid,
								LastUpdatedDate = GETDATE()
								where ProviderId = @providerId;
							end
							else if(@paymentStatus = 'disapproved')
							begin
								set @providerBalance = @amount + @currentBalance;
								set @WithdrawalPending = @currentWithdrawalPending - @amount;
								Update ProviderWallet
								set  ProviderBalance = @providerBalance,
								WithdrawalPending = @WithdrawalPending,
								LastUpdatedDate = GETDATE()
								where ProviderId = @providerId;
							end
							else if(@paymentStatus = 'refunded')
							begin
								set @providerBalance = @currentBalance - @amount;
								update ProviderWallet set ProviderBalance = @providerBalance, LastUpdatedDate = GetDate();
							end";
			return query.Replace("@", "@@");
		}

		public string ExistsTri_ProviderWallet_DebitBalance()
		{
			return @"SELECT count(*) FROM sysobjects WHERE  name = 'tri_ProviderWallet_DebitBalance'";
		}
		public string Tri_ProviderWallet_DebitBalance()
		{
			var query = @"Create Trigger [dbo].[tri_ProviderWallet_DebitBalance] on [dbo].[ProviderPaymentLogs]
							after Insert
							As
								declare @providerId int;
								declare @providerBalance decimal(19,4)
								declare @currentBalance decimal(19,4)
								declare @currentWithdrawal decimal(19,4)
								declare @paymentStatus nvarchar(max);
								declare @WithdrawalPending decimal(19,4)
								declare @amount decimal(19,4)

								select @providerId = i.ProviderId, @amount = i.Amount,@paymentStatus = i.PaymentStatus 
								from inserted i;

								select @currentBalance = ProviderBalance,
								@currentWithdrawal = (case when WithdrawalPending is null then 0 else WithdrawalPending end)
								from ProviderWallet where ProviderId = @providerId;
	
	

							  if(@paymentStatus = 'withdrawalpending')
								begin
									set @providerBalance = @currentBalance - @amount;
									set @WithdrawalPending = @amount + @currentWithdrawal;

									Update ProviderWallet
									set  ProviderBalance = @providerBalance,
									WithdrawalPending = @WithdrawalPending,
									LastUpdatedDate = GETDATE()
									where ProviderId = @providerId;
								end";
			return query.Replace("@", "@@");
		}
	}
}