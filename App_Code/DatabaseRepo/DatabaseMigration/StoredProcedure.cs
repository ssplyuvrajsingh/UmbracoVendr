namespace UmbracoVendr.App_Code.DatabaseRepo.DatabaseMigration
{
	public class StoredProcedure
	{
		public string ExistSpProductTableQuery()
		{
			return "SELECT count(*) FROM sysobjects WHERE  name = 'spProductsTable'";
		}
		public string SpProductTableQuery()
		{
			string query = @"Create procedure [dbo].[spProductsTable]  
				(  
				@Action int = 0,
				@ProductId int = 0,
				@MemberId int = 0,
				@ProductName nvarchar(MAX) = null,
				@ProductType nvarchar(MAX) = null,
				@ProductPrice float = 0.0,
				@ProductDescription nvarchar(MAX) = null,
				@ProductFiles nvarchar(MAX) = null,
				@ProductCreation datetime = null,
				@ProductLogoLarge nvarchar(MAX) = null,
				@ProductLogoSmall nvarchar(MAX)	= null,
				@ProductScreenshot nvarchar(MAX) = null,
				@ProductScreenshotDescription nvarchar(MAX) = null,
				@ProductProtectionTrialLength int = 0,
				@ProductProtectionNumberOfTrials int = 0,
				@ProductStatus nvarchar(MAX) = null,
				@ProductNodeId int = 0,
				@ProviderId int = 0,
				@ProductVersionNumber nvarchar(Max) = null,
				@ProductFileId int =0,
				@CreatedVersionDateTime DateTime = null,
				@ProductNodeKey nvarchar(max) = null,
				@OrderId nvarchar(max) = null,
				@OrderStatusId nvarchar(max) = null
				)
				as 


				--Insert New Product and Add new version
				if(@Action=1)
				begin  

				Insert into ProductsTable(MemberId,ProductName,ProductType,ProductPrice,ProductDescription,ProductCreation,ProductLogoLarge,ProductLogoSmall,ProductScreenshot,ProductScreenshotDescription,ProductProtectionTrialLength,ProductProtectionNumberOfTrials,ProductStatus)
				values (@MemberId,@ProductName,@ProductType,@ProductPrice,@ProductDescription,@ProductCreation,@ProductLogoLarge,@ProductLogoSmall,@ProductScreenshot,@ProductScreenshotDescription,@ProductProtectionTrialLength,@ProductProtectionNumberOfTrials,@ProductStatus)

				declare @Product_Id int = @@identity

				Create Table #TempProductIds(ProductFile nvarchar(Max)) 
				insert into #TempProductIds SELECT * FROM STRING_SPLIT (@ProductFiles, ',')

				insert into ProductFilesTable (ProductId, ProductFile) SELECT @Product_Id, ProductFile FROM #TempProductIds
				drop table #TempProductIds
	
				End

				--Get Products by Member ID
				if(@Action=2)
				begin  

				select * from ProductsTable where MemberId = @MemberId order by ProductCreation desc;

				End

				--Get Product Details by Product ID
				if(@Action=3)
				begin  

				--select * from ProductsTable where ProductId = @ProductId
				select pt.*, 
				STUFF((select ','+ProductFile from ProductFilesTable where ProductId=pt.ProductId FOR XML PATH('')),1,1,'') as ProductFiles 
				from ProductsTable pt
				where pt.ProductId=@ProductId

				End

				--Get Product Files by ProductId
				if(@Action=4)
				begin  

				select * from ProductFilesTable where ProductId = @ProductId

				End

				--Update Product by Product ID
				if(@Action=5)
				begin  
				if(@ProductFiles is not null)
				begin
				Update ProductsTable SET ProductName=@ProductName, ProductType=@ProductType, ProductPrice=@ProductPrice, ProductDescription=@ProductDescription, ProductLogoLarge=@ProductLogoLarge, 
					ProductLogoSmall=@ProductLogoSmall, ProductScreenshot=@ProductScreenshot, ProductScreenshotDescription=@ProductScreenshotDescription, 
					ProductProtectionTrialLength=@ProductProtectionTrialLength, ProductProtectionNumberOfTrials=@ProductProtectionNumberOfTrials, ProductStatus=@ProductStatus 
					WHERE ProductId = @ProductId

				--Delete from ProductFilesTable where ProductId = @ProductId

				Create Table #TempUpdateProductIds(ProductFile nvarchar(Max)) 
				insert into #TempUpdateProductIds SELECT * FROM STRING_SPLIT (@ProductFiles, ',')

				insert into ProductFilesTable (ProductId, ProductFile, ProductVersionNumber) SELECT @ProductId, ProductFile,@ProductVersionNumber FROM #TempUpdateProductIds
				drop table #TempUpdateProductIds
				end
				else
				begin
				Update ProductsTable SET ProductName=@ProductName, ProductType=@ProductType, ProductPrice=@ProductPrice, ProductDescription=@ProductDescription, ProductLogoLarge=@ProductLogoLarge, 
					ProductLogoSmall=@ProductLogoSmall, ProductScreenshot=@ProductScreenshot, ProductScreenshotDescription=@ProductScreenshotDescription, 
					ProductProtectionTrialLength=@ProductProtectionTrialLength, ProductProtectionNumberOfTrials=@ProductProtectionNumberOfTrials, ProductStatus=@ProductStatus 
					WHERE ProductId = @ProductId
				end
				End
				--Get All Products
				if(@Action=6)
				begin  

				select * from ProductsTable

				End

				--Update Product Status
				if(@Action=7)
				begin  

				UPDATE ProductsTable SET ProductStatus = @ProductStatus, ProductNodeId = @ProductNodeId, ProductNodeKey = @ProductNodeKey where ProductId = @ProductId

				End

				--Get All Products by Product Category and ProductStatus Active 
				if(@Action=8)
				begin  

				select * from ProductsTable where ProductType = @ProductType and ProductStatus = 'Active'

				End

				--Get All Products of provider.
				if(@Action=9)
				begin  

				select * from ProductsTable where MemberId = @ProviderId

				End

				--Get Product Details by Product Node ID
				if(@Action=10)
				begin  

				--select * from ProductsTable where ProductId = @ProductId
				select pt.*, 
				STUFF((select ','+ProductFile from ProductFilesTable where ProductId=pt.ProductId FOR XML PATH('')),1,1,'') as ProductFiles 
				from ProductsTable pt
				where pt.ProductNodeId=@ProductNodeId

				End

				--Get Product Files
				if(@Action=11)
				begin  
				select * from ProductFilesTable  where ProductId =  @ProductId
				End

				--Delete Product Files
				if(@Action=12)
				begin  
				Delete ProductFilesTable where ProductFileId = @ProductFileId
				select count(*) from ProductFilesTable where ProductVersionNumber = @ProductVersionNumber
				End

				if(@Action = 13)
				begin
				Update ProductsTable set ProductStatus = @ProductStatus where ProductId = @ProductId
				Create Table #Temp1ProductIds(ProductFile nvarchar(Max)) 
				insert into #Temp1ProductIds SELECT * FROM STRING_SPLIT (@ProductFiles, ',')

				insert into ProductFilesTable (ProductId, ProductFile, ProductVersionNumber, CreatedVersionDateTime) SELECT @ProductId, ProductFile, @ProductVersionNumber,@CreatedVersionDateTime FROM #Temp1ProductIds
				drop table #Temp1ProductIds
				end

				--Get All Products and Porduct Status Active
				if(@Action=14)
				begin  
				select * from ProductsTable where ProductStatus = 'Active'
				End

				--Get All order list according provider Id
				if(@Action = 15)
				begin
				Select @ProductNodeKey =  STUFF((select ','+ProductNodeKey from ProductsTable where MemberId = @MemberId FOR XML PATH('')),1,1,'')

				select VO.customerFirstName , VO.customerLastName, vOL.quantity , VO.amountAuthorized,
				(select name from vendrOrderStatus where id = VO.orderStatusId) as orderStatus,
				(select color from vendrOrderStatus where id = VO.orderStatusId) as orderColor,
				(CASE
				WHEN VO.paymentStatus = 0 THEN 'Initialized'
				WHEN VO.paymentStatus = 1 THEN 'Authorized'
				WHEN VO.paymentStatus = 2 THEN 'Captured'
				WHEN VO.paymentStatus = 3 THEN 'Cancelled'
				WHEN VO.paymentStatus = 4 THEN 'Refunded'
				WHEN VO.paymentStatus = 5 THEN 'Pending'
				WHEN VO.paymentStatus = 200 THEN 'Error'
				END) as paymentStatus,
				vOL.orderId,
				VO.createDate
				from vendrOrderLine as vOL join vendrOrder as VO 
				on vOL.orderId = VO.id  
				where vOL.productReference in (SELECT value FROM STRING_SPLIT(@ProductNodeKey, ','))
				and VO.finalizedDate is not null
				order by VO.createDate desc;
				end

				--Get All order status
				if(@Action = 16)
				begin
				select orderStatusId from vendrOrder where id= @OrderId;
				select Id,Name from vendrOrderStatus;
				end

				--Update Order Status by Order Id
				if(@Action = 17)
				begin
				Update vendrOrder set orderStatusId = @OrderStatusId where id = @OrderId;
				end";
			return query.Replace("@", "@@");
		}
		public string ExistSpProductWallet()
		{
			return "SELECT count(*) FROM sysobjects WHERE  name = 'spProductWallet'";
		}
		public string SpProductWallet()
		{
			string query = @"Create proc [dbo].[spProductWallet]
						(
						@Action int = 0,
						@ProviderId int = 0,
						@Amount decimal(19,4)=0,
						@PaymentStatus nvarchar(max) = null,
						@LogId int = 0,
						@OrderId nvarchar(max) = null,
						@ProviderBalance decimal(19,4) = 0,
						@BankAccountNumber nvarchar(max) = null,
						@IFSC nvarchar(20) = null,
						@AccountHolderName nvarchar(max) = null,
						@PassCode nvarchar(20) = null
						)
						as
						if(@Action = 1)
							begin
								Select top 1 * from ProviderWallet where ProviderId = @ProviderId;
							end

						--Post Withdrawal Request
						if(@Action = 2)
							begin
								select @ProviderBalance = ProviderBalance from ProviderWallet where ProviderId = @ProviderId;

								if(@ProviderBalance >= @Amount)
								begin
									Insert ProviderPaymentLogs(ProviderId,Description,Amount,LogDate,PaymentStatus,OrderId)
									values(@ProviderId,'Withdrawal by provider',@Amount,GETDATE(),'withdrawalpending',@OrderId)
								end
							end

						--Get Withdrawal Requests List
						if(@Action = 3)
						begin
							select LogId,ProviderId,Description,Amount,LogDate,OrderId,
							(case  when PaymentStatus = 'withdrawalpending' then 'Withdrawal pending' end) as PaymentStatus
							from ProviderPaymentLogs where PaymentStatus in ('withdrawalpending');
						end

						--Update Payment Status
						if(@Action = 4)
						begin
							Update ProviderPaymentLogs set PaymentStatus=@PaymentStatus where LogId = @LogId;
						end

						--Get Payment History By Provider
						if(@Action = 5)
						begin
							select Description,Amount,LogDate,OrderId,
							(case
								when PaymentStatus = 'withdrawalpending' then 'Withdrawal pending' 
								when PaymentStatus = 'uncaptured' then 'Uncaptured'
								when PaymentStatus = 'captured' then 'Captured'
								when PaymentStatus = 'approved' then 'Successfully withdrawal'
								when PaymentStatus = 'disapproved' then 'Disapproved by admin'
								when PaymentStatus = 'refunded' then 'Refunded payment to the customer'
							end) as PaymentStatus
							from ProviderPaymentLogs where ProviderId = @ProviderId and PaymentStatus != 'uncaptured'
							order by LogDate desc;
						end

						--Get Payment History By Admin
						if(@Action = 6)
						begin
							select LogId,ProviderId,Description,Amount,LogDate,OrderId,
							(case
								when PaymentStatus = 'approved' then 'Successfully withdrawal'
								when PaymentStatus = 'disapproved' then 'Disapproved by admin'
							end) as PaymentStatus
							from ProviderPaymentLogs where PaymentStatus in ('approved','disapproved');
						end

						--Edit and Save Bank Account Details
						if(@Action = 7)
						begin
							if exists(Select 1 from ProviderBankDetails where ProviderId = @ProviderId)
							begin
								Update ProviderBankDetails Set
								BankAccountNumber = @BankAccountNumber, IFSC = @IFSC, AccountHolderName = @AccountHolderName,
								 LastUpdatedDate = GETDATE() where ProviderId = @ProviderId;
							end
							else
							begin
								Insert ProviderBankDetails(ProviderId,BankAccountNumber,IFSC,AccountHolderName,LastUpdatedDate)
								Values(@ProviderId,@BankAccountNumber,@IFSC,@AccountHolderName,GETDATE());
							end
						end

						--Get Provider Bank Details
						if(@Action = 8)
							begin
								Select top 1 * from ProviderBankDetails where ProviderId = @ProviderId;
							end

						--Get Provider Withdrawal Details By Admin
						if(@Action = 9)
							begin
								select ppl.LogId,ppl.ProviderId,ppl.Amount,ppl.OrderId,pbd.BankAccountNumber,pbd.IFSC,pbd.AccountHolderName from ProviderPaymentLogs ppl
								join ProviderBankDetails pbd on pbd.ProviderId = ppl.ProviderId
								where pbd.ProviderId = @ProviderId and ppl.LogId =@LogId;
							end
							";
			return query.Replace("@", "@@"); ;
		}
	}
}