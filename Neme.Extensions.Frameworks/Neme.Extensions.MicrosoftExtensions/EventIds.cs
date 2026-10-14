namespace Neme.Extensions.MicrosoftExtensions;

internal static class EventIds
{
    private const string _prefix = "Neme.Extensions.MicrosoftExtensions.";

    public static class FileCache
    {
        private const string _classPrefix = _prefix + nameof(FileCache) + ".";

        public const int CachedKey = 12000;
        public const int RemovedCacheKey = 12001;
        public const int FailedToDeleteCacheFile = 12002;
        public const int CacheCleared = 12003;
        public const int FailedToCleanupCacheFile = 12004;
        public const int CleanedUpExpiredEntries = 12005;
        public const int CleanedUpUnusedLocks = 12006;
        public const int RefreshedSlidingExpiration = 12007;
        public const int MetadataMissingForFile = 12008;

        public const string CachedKeyName = _classPrefix + nameof(CachedKey);
        public const string RemovedCacheKeyName = _classPrefix + nameof(RemovedCacheKey);
        public const string FailedToDeleteCacheFileName = _classPrefix + nameof(FailedToDeleteCacheFile);
        public const string CacheClearedName = _classPrefix + nameof(CacheCleared);
        public const string FailedToCleanupCacheFileName = _classPrefix + nameof(FailedToCleanupCacheFile);
        public const string CleanedUpExpiredEntriesName = _classPrefix + nameof(CleanedUpExpiredEntries);
        public const string CleanedUpUnusedLocksName = _classPrefix + nameof(CleanedUpUnusedLocks);
        public const string RefreshedSlidingExpirationName = _classPrefix + nameof(RefreshedSlidingExpiration);
        public const string MetadataMissingForFileName = _classPrefix + nameof(MetadataMissingForFile);
    }

    public static class FileCacheCleanupService
    {
        private const string _classPrefix = _prefix + nameof(FileCacheCleanupService) + ".";

        public const int FileCacheCleanupStarting = 12009;
        public const int FileCacheCleanupStopping = 12010;

        public const string FileCacheCleanupStartingName = _classPrefix + nameof(FileCacheCleanupStarting);
        public const string FileCacheCleanupStoppingName = _classPrefix + nameof(FileCacheCleanupStopping);
    }
}
