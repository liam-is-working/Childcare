USE [ChildCareSystemDb]
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'310532c1-2d29-496d-bba8-05827298e1e1', N'Manager', N'MANAGER', N'06f00422-fc4a-4c87-aeba-8e753952c187')
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'523dc856-e7d8-4af8-b849-45322d76391f', N'Admin', N'ADMIN', N'ca4a8d47-a158-4a63-8f36-5bc63a1d44a4')
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'd3d9ed02-660a-445c-bd93-ccc2e860b2f3', N'Staff', N'STAFF', N'84943f85-689c-4ece-ae34-5117a7c944c9')
GO
INSERT [dbo].[AspNetUsers] ([ChildCareUserId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [Address], [CitizenID], [DOB], [FullName]) VALUES (N'c6eacd72-46d2-4819-a401-dcfa0fbc6ef6', N'manager@gmail.com', N'MANAGER@GMAIL.COM', N'manager@gmail.com', N'MANAGER@GMAIL.COM', 1, N'AQAAAAEAACcQAAAAEOfcOo6ZlzfW6xDUeM3U0QU3I0I04zaDbCYgAwxu7VL3oHP2sCxZ8xodc+24B1YJ9Q==', N'M6VF7AJKIZ7O4Q2GLXTN6QGLFBO24KCG', N'bcb3a925-aa1d-42c9-8edb-0a5097b77b79', NULL, 0, 0, NULL, 1, 0, N'FPT HCM', N'00000000000', CAST(N'2001-07-14T00:00:00.0000000' AS DateTime2), N'Manager')
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'c6eacd72-46d2-4819-a401-dcfa0fbc6ef6', N'310532c1-2d29-496d-bba8-05827298e1e1')
GO
SET IDENTITY_INSERT [dbo].[Managers] ON 

INSERT [dbo].[Managers] ([ManagerID], [ChildcareUserId]) VALUES (2, N'c6eacd72-46d2-4819-a401-dcfa0fbc6ef6')
SET IDENTITY_INSERT [dbo].[Managers] OFF
GO
SET IDENTITY_INSERT [dbo].[Statuses] ON 

INSERT [dbo].[Statuses] ([StatusID], [StatusName]) VALUES (1, N'Approved')
INSERT [dbo].[Statuses] ([StatusID], [StatusName]) VALUES (2, N'Rejected')
INSERT [dbo].[Statuses] ([StatusID], [StatusName]) VALUES (3, N'Pending')
SET IDENTITY_INSERT [dbo].[Statuses] OFF
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20210627050225_Identity', N'3.1.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20210627094640_CustomUserAttribute', N'3.1.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20210627235902_initDB', N'3.1.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20210628000138_InitDatabase', N'3.1.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20210628000924_InitFix', N'3.1.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20210628001717_testAdmin', N'3.1.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20210628002005_test', N'3.1.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20210628003217_fixRela', N'3.1.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20210628003550_fixRela2', N'3.1.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20210628004223_recreate', N'3.1.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20210628010948_recre', N'3.1.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20210706093540_reservation, service, reservationTime', N'3.1.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20210711034041_fixReservationTime', N'3.1.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20210712141013_addSpecIdtoReservation', N'3.1.0')
GO
