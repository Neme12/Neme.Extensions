#include <fcntl.h>
#include <stdint.h>
#include <sys/stat.h>

struct neme_stat
{
	int64_t size;
	uint32_t mode;
	uint32_t uid;
	uint32_t gid;
	int64_t atime_seconds;
	int64_t atime_nanoseconds;
	int64_t mtime_seconds;
	int64_t mtime_nanoseconds;
	int64_t birthtime_seconds;
	int64_t birthtime_nanoseconds;
	uint32_t flags;
};

int neme_fcntl_getpath(int fd, char* path)
{
	return fcntl(fd, F_GETPATH, path);
}

int neme_fstat(int fd, struct neme_stat* result)
{
	struct stat s;
	int rv = fstat(fd, &s);
	if (rv == 0)
	{
		result->size = (int64_t)s.st_size;
		result->mode = (uint32_t)s.st_mode;
		result->uid = (uint32_t)s.st_uid;
		result->gid = (uint32_t)s.st_gid;
		result->atime_seconds = (int64_t)s.st_atimespec.tv_sec;
		result->atime_nanoseconds = (int64_t)s.st_atimespec.tv_nsec;
		result->mtime_seconds = (int64_t)s.st_mtimespec.tv_sec;
		result->mtime_nanoseconds = (int64_t)s.st_mtimespec.tv_nsec;
		result->birthtime_seconds = (int64_t)s.st_birthtimespec.tv_sec;
		result->birthtime_nanoseconds = (int64_t)s.st_birthtimespec.tv_nsec;
		result->flags = (uint32_t)s.st_flags;
	}

	return rv;
}
