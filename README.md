# RedditTracker
You will need Reddit API key and Reddit account username and password to run this.
Optionally if database storage is needed publish database and setup connection string as well.
These need to be setupo in appsettings.json, please see below.

"dbConnection": "", // "Data Source=DESKTOP-ET8S6AP\\MSSQLSERVER1;Database=dbCano;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=30070;Encrypt=False;TrustServerCertificate=False",
  "ApiKeys": {
    "clientId": "--clientId here--",
    "clientSecret": "--clientsecret here--"
  },
  "RedditAccount": {
    "username": "--Reddit username--",
    "password": "--Reddit password--"
  }
