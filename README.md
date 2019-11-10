# MiniBank Website
A simple online banking simulation website created using ASP.NET Framework with MVC (No focus on design)  

![alt text](https://raw.githubusercontent.com/aesmailsparta/MiniBank_ASP.NET_Framework_Website/master/Screenshots/MiniBankOverview_Thumbnail.png "MiniBank Screenshots All")  

#### Technologies
C#, Javascript, CSS, HTML

#### Features
* Login/Registration
* Verification email sent out on successful registration
* Resend activation email, if requested
* Secure forgot password reset using an OTP (One Time Password) system
* Live form validation before submit
* Open a new bank account for a customer
* See breakdown of bank account activity on home dashboard
* View transactions for each account
* Transfer money between a customers own accounts
* Create and save Payees to transfer money to
* Transfer money to Payees that have been created
* Remove Payees that are no longer needed


#### Code Highlights

Data validation example using data annotations
With a custom validation attribute created which makes sure the date selected is not a future date
```c#
```

Small sample from the forgot password code, which shows how the OTP is created and sent out
```c#
```


#### ScreenShots

![alt text](https://raw.githubusercontent.com/aesmailsparta/MiniBank_ASP.NET_Framework_Website/master/Screenshots/Dashboard_Overview.png "MiniBank Screenshot - Dashboard Overview")  

![alt text](https://raw.githubusercontent.com/aesmailsparta/MiniBank_ASP.NET_Framework_Website/master/Screenshots/Accounts.png "MiniBank Screenshot - Accounts Dashboard")  

![alt text](https://raw.githubusercontent.com/aesmailsparta/MiniBank_ASP.NET_Framework_Website/master/Screenshots/TransferMoney.png "MiniBank Screenshot - Money Transfer")  

![alt text](https://raw.githubusercontent.com/aesmailsparta/MiniBank_ASP.NET_Framework_Website/master/Screenshots/Login(Validation).png "MiniBank Screenshot - Customer Login (With Validation)")
