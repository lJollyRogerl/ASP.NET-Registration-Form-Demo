create database RunetSoftDb
go

use  RunetSoftDb 
go

create table tblUsers
(
	UserID int identity(1,1) primary key,
	UserName nvarchar(150),
	Email varchar(150),
	DateOfBirth datetime,
	Password nvarchar(MAX),
	Country nvarchar(50),
	IsEmailVerified bit,
	ActivationCode uniqueidentifier
)