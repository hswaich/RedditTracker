create table dbo.SubredditTracker
(
	Id			int	not null primary key identity(1,1),
	Subreddit	nvarchar(50)	not null,
	Message		nvarchar(500)	not null,
	OccurredOn	datetime		not null default getdate()
)
