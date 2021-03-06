using BadgeUpClient.Http;
using BadgeUpClient.ResourceClients;

namespace BadgeUpClient
{
	/// <summary>
	/// BadgeUp Client
	/// </summary>
	public class BadgeUpClient : BadgeUpClientInterface, System.IDisposable
	{
		const string DEFAULT_HOST = "https://api.useast1.badgeup.io";
		protected string m_host;
		protected ApiKey m_apiKey;
		protected BadgeUpHttpClient m_httpClient;

		// resource clients


		/// <summary>
		/// Interact with BadgeUp accounts
		/// </summary>
		public AccountClient Account;

		/// <summary>
		/// Interact with BadgeUp achievements
		/// </summary>
		public AchievementClient Achievement;
		
		/// <summary>
		/// Interact with BadgeUp achievementIcons
		/// </summary>
		public AchievementIconClient AchievementIcon;

		/// <summary>
		/// Interact with BadgeUp applications
		/// </summary>
		public ApplicationClient Application;

		/// <summary>
		/// Interact with BadgeUp awards
		/// </summary>
		public AwardClient Award;

		/// <summary>
		/// Interact with BadgeUp criteria
		/// </summary>
		public CriterionClient Criterion;

		/// <summary>
		/// Interact with BadgeUp earned achievements
		/// </summary>
		public EarnedAchievementClient EarnedAchievement;

		/// <summary>
		/// Interact with BadgeUp events
		/// </summary>
		public EventClient Event;
		
		/// <summary>
		/// Interact with BadgeUp metrics
		/// </summary>
		public MetricClient Metric;

		/// <summary>
		/// Instantiate the BadgeUpClient, providing an instance of <see cref="ApiKey"/>
		/// </summary>
		/// <param name="apiKey">API key generated from the BadgeUp dashboard</param>
		/// <param name="host">Optional. BadgeUp instance to use.</param>
		public BadgeUpClient(ApiKey apiKey, string host)
		{
			this.m_apiKey = apiKey;
			this.m_host = host;

			this.m_httpClient = new BadgeUpHttpClient(apiKey, host);

			this.Account = new AccountClient(this.m_httpClient);
			this.Achievement = new AchievementClient(this.m_httpClient);
			this.AchievementIcon = new AchievementIconClient(this.m_httpClient);
			this.Application = new ApplicationClient(this.m_httpClient);
			this.Award = new AwardClient(this.m_httpClient);
			this.Criterion = new CriterionClient(this.m_httpClient);
			this.EarnedAchievement = new EarnedAchievementClient(this.m_httpClient);
			this.Event = new EventClient(this.m_httpClient);
			this.Metric = new MetricClient(this.m_httpClient);
		}

		/// <summary>
		/// Instantiate the BadgeUpClient, providing an apiKey
		/// </summary>
		/// <param name="apiKey">API key generated from the BadgeUp dashboard</param>
		/// <param name="host">Optional. BadgeUp instance to use.</param>
		public BadgeUpClient(string apiKey, string host = DEFAULT_HOST)
			: this(ApiKey.Create(apiKey), host)
		{
		}

		// for test purposes only
		public void _SetHttpClient(System.Net.Http.HttpClient h)
		{
			this.m_httpClient._SetHttpClient(h);
		}

		public void Dispose()
		{
			m_httpClient.Dispose();
		}
	}
}
