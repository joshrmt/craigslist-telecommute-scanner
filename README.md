craigslist-telecommute-scanner
==============================

C# Automated Craigslist telecommute job scanner/aggregator.  This software can be used to locate telecommute software jobs on Craigslist.

Scanning can take some time, so this project is setup to be deployed on Microsoft Azure using a web role and worker role.

Projects:

 - CraigslistScanner.Data (Entity Framework project to save jobs to SQL.  Just set CraigslistDatabase.ConnectionString to your SQL connection string).
 - CraigslistScanner.ScannerRole (Azure worker role that will locate telecommute jobs)
 - CraigslistScanner.CLWebViewRole (Azure web role to view the results of the scanning process)