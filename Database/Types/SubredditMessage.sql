create type dbo.SubredditMessage as table
(
	Subreddit	nvarchar(50)	not null,
	Message		nvarchar(500)	not null
)
