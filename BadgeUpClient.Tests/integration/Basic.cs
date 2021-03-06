using Xunit;
using BadgeUpClient.Types;
using BadgeUpClient.Responses;

namespace BadgeUpClient.Tests
{
	public class BasicIntegration
	{
		// get a real API Key for integration testing
		string API_KEY = System.Environment.GetEnvironmentVariable("INTEGRATION_API_KEY");

		[SkippableFact]
		public async void BasicIntegration_SendEvent()
		{
			if (string.IsNullOrEmpty(API_KEY))
				throw new SkipException("Tests skipped on environments without API_KEY variable configured");

			var client = new BadgeUpClient(API_KEY);
			System.Random rand = new System.Random();
			string subject = "dotnet-ci-" + rand.Next(100000);
			string key = "test";
			Event @event = new Event(subject, key, new Modifier {Inc = 5});

			EventResponse result = await client.Event.Send(@event);

			// sanity check inputs
			Assert.Equal(key, result.Event.Key);
			Assert.Equal(subject, result.Event.Subject);
			Assert.Equal(5, result.Event.Modifier.Inc);

			// expect the achievemnt to be earned
			Assert.Single(result.Progress);
			Assert.True(result.Progress[0].IsComplete);
			Assert.True(result.Progress[0].IsNew);
			Assert.Equal(1, result.Progress[0].PercentComplete);

			foreach (var prog in result.Progress)
			{
				if (prog.IsComplete && prog.IsNew)
				{
					string earnedAchievementId = prog.EarnedAchievementId;
					string achievementId = prog.AchievementId;
					System.Console.WriteLine($"Achievement with ID {prog.AchievementId} Earned!");

					// from here you can use AchievementId and EarnedAchievementId to get the original achievement and awards objects
					var earnedAchievement = await client.EarnedAchievement.GetById(earnedAchievementId);
					var achievement = await client.Achievement.GetById(achievementId);

					// get associated award information
					foreach (var awardId in achievement.Awards)
					{
						var award = await client.Award.GetById(awardId);
						int points = award.Data["points"].ToObject<int>();
						System.Console.WriteLine($"Points awarded: {points}");
					}

					//get associated criteria
					foreach (var criterion in prog.ProgressTree.Criteria)
					{
						var criterionRespone = await client.Criterion.GetById(criterion.Key);
						Assert.Equal(criterion.Key, criterionRespone.Id);
					}
				}
			}


			var apiKey = ApiKey.Create(API_KEY);
			var metric = await client.Metric.GetIndividualBySubject(@event.Subject, key);

			Assert.Equal(apiKey.ApplicationId, metric.ApplicationId);
			Assert.Equal(@event.Key, metric.Key);
			Assert.Equal(@event.Subject, metric.Subject);
			Assert.True(metric.Value >= @event.Modifier.Inc);

			// var progress = result.Progress[0];

			// var earnedAchievement = await client.EarnedAchievement.GetById(progress.EarnedAchievementId);
			// Assert.Equal(progress.EarnedAchievementId, earnedAchievement.Id);

			// var achievement = await client.Achievement.GetById(result.Progress[0].AchievementId);
			// Assert.Equal(progress.AchievementId, achievement.Id);

			// var award = await client.Award.GetById(achievement.Awards[0]);
			// Assert.Equal(achievement.Awards[0], award.Id);
			// Assert.NotNull(award.Data);
			// Assert.Equal(5, award.Data["points"]);
		}

		[SkippableFact]
		public async void BasicIntegration_GetApplication()
		{
			if (string.IsNullOrEmpty(API_KEY))
				throw new SkipException("Tests skipped on environments without API_KEY variable configured");

			var client = new BadgeUpClient(API_KEY);
			var apiKey = ApiKey.Create(API_KEY);

			var application = await client.Application.GetById(apiKey.ApplicationId);

			Assert.Equal(apiKey.ApplicationId, application.Id);
			Assert.Equal(apiKey.AccountId, application.AccountId);
		}

		[SkippableFact]
		public async void BasicIntegration_GetAccount()
		{
			if (string.IsNullOrEmpty(API_KEY))
				throw new SkipException("Tests skipped on environments without API_KEY variable configured");

			var client = new BadgeUpClient(API_KEY);
			var apiKey = ApiKey.Create(API_KEY);

			var account = await client.Account.GetById(apiKey.AccountId);

			Assert.Equal(apiKey.AccountId, account.Id);
		}

		[SkippableFact]
		public async void BasicIntegration_GetIcons()
		{
			if (string.IsNullOrEmpty(API_KEY))
				throw new SkipException("Tests skipped on environments without API_KEY variable configured");

			var client = new BadgeUpClient(API_KEY);
			var apiKey = ApiKey.Create(API_KEY);

			//some achievement icons have to be uploaded for the test to pass.
			var icons = await client.AchievementIcon.GetAll();
			Assert.False(icons.Length == 0);
		}

		[SkippableFact]
		public async void BasicIntegration_GetAllMetrics()
		{
			if (string.IsNullOrEmpty(API_KEY))
				throw new SkipException("Tests skipped on environments without API_KEY variable configured");

			var client = new BadgeUpClient(API_KEY);

			//there should be more then 50 metrics created, for the default page size of API is 50 elements, and we want to check multiple page retrieval 
			var metrics = await client.Metric.GetAll();
			Assert.True(metrics.Count > 50);
		}
	}
}
