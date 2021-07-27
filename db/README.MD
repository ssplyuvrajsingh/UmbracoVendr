			*****How To Restore A SQL Server Backup File*****

Step 1 :- Right clicking the "Database" branch of the SQL Server Management Studio and Click on "Restore Database"
		![See Screenshot](https://bitbucket.org/fintechnology/umbracovendr/src/master/db/StepsScreenshoots/Step1.png)

Step 2 :- Now aapear a box, Here choose "Device" option. 
		![See Screenshot](https://bitbucket.org/fintechnology/umbracovendr/src/master/db/StepsScreenshoots/Step2.png)

Step 3 :- Now provide source file path or clicking on three dots[...]
		![See Screenshot](https://bitbucket.org/fintechnology/umbracovendr/src/master/db/StepsScreenshoots/Step3.png)
		
Step 4 :- Now enter your destination file name. (Note -: Allready should not exists your destination file So Don't create an empty database and restore the .bak file on to it.)
		![See Screenshot](https://bitbucket.org/fintechnology/umbracovendr/src/master/db/StepsScreenshoots/Step4.png)	

			*****End*****
			
   *****Set Your Connection String*****
   
-Set your connection string in webconfig file, here enter your server, database, id and password
		![See Screenshot](https://bitbucket.org/fintechnology/umbracovendr/src/master/db/StepsScreenshoots/SetupConnectionString.png)	
	
	*****End*****

	******Package Triggers For manage provider wallet********

Table Name   	- 	VendrOrderLine
Trigger Name 	- 	[tri_ProviderPaymentLogs_Insert_When_GetOrder], [tri_ProviderPaymentLogs_When_DeleteOrder]

Table Name   	- 	VendrOrder
Trigger Name 	-	[tri_ProviderPaymentLogs_Update_When_OrderStatusCaptured]

Table Name   	- 	[ProviderPaymentLogs]
Trigger Name 	-	[tri_ProviderWallet_AddBalance], [tri_ProviderWallet_DebitBalance]

	*******End********
