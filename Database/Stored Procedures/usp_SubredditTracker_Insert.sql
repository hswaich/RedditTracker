create procedure dbo.usp_SubredditTracker_Insert
	@subredditMessages	dbo.SubredditMessage readonly
as
begin
	insert into dbo.SubredditTracker
	(
		Subreddit,
		[Message]
	)
	select	Subreddit,
			[Message]
	from	@subredditMessages;
end