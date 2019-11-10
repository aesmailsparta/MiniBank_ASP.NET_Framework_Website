# MiniBank Website
A simple online banking simulation website created using ASP.NET Framework with MVC  

![alt text](https://raw.githubusercontent.com/aesmailsparta/MiniBank_ASP.NET_Framework_Website/master/Screenshots/MiniBankOverview_Thumbnail.png "MiniBank Screenshots All")  

## Technologies
C#, Javascript, CSS, HTML

## Features
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


## Code Highlights

#### Overview

###### HomeController -> Logged in user accessible pages and data output
###### User Contoller -> User management i.e. login, sign up, resend validation, forgot password
###### AccountController -> Bank account management i.e. money transfer, open new account
###### PayeeController -> Payee management i.e. CRUD functions for a Payee  

#### Data Validation 
Here you can see how i used data annotations for input validation
There is also a custom validation attribute I created which makes sure the date selected is not a future date  

[Transaction Data Model With Data Annotations Example](https://github.com/aesmailsparta/MiniBank_ASP.NET_Framework_Website/blob/master/MiniBank/MiniBank/Models/TransactionData.cs "Data Annotations")  

[User Model With Custom Data Annotation Example](https://github.com/aesmailsparta/MiniBank_ASP.NET_Framework_Website/blob/master/MiniBank/MiniBank/Models/Extended/User.cs "Custom Data Annotations")  

#### Forgot Password OTP
You can find the logic for my forgot password OTP code on *lines 284 - 449*, which shows how the OTP is created and sent out.
*Lines 505 - 553* in the same controller is the function for the actual sending of the email  

[User Model With Custom Data Annotation Example](https://github.com/aesmailsparta/MiniBank_ASP.NET_Framework_Website/blob/master/MiniBank/MiniBank/Controllers/UserController.cs "Custom Data Annotations")  


## ScreenShots

![alt text](https://raw.githubusercontent.com/aesmailsparta/MiniBank_ASP.NET_Framework_Website/master/Screenshots/Dashboard_Overview.png "MiniBank Screenshot - Dashboard Overview")  

![alt text](https://raw.githubusercontent.com/aesmailsparta/MiniBank_ASP.NET_Framework_Website/master/Screenshots/Accounts.png "MiniBank Screenshot - Accounts Dashboard")  

![alt text](https://raw.githubusercontent.com/aesmailsparta/MiniBank_ASP.NET_Framework_Website/master/Screenshots/TransferMoney.png "MiniBank Screenshot - Money Transfer")  

![alt text](https://raw.githubusercontent.com/aesmailsparta/MiniBank_ASP.NET_Framework_Website/master/Screenshots/Login(Validation).png "MiniBank Screenshot - Customer Login (With Validation)")
