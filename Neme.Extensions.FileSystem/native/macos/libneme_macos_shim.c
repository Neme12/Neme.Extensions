#include <fcntl.h>
#include <stdint.h>
#include <sys/stat.h>

int neme_fcntl_getpath(int fd, char* path)
{
	return fcntl(fd, F_GETPATH, path);
}

int neme_fstat_flags(int fd, uint32_t* flags)
{
	struct stat s;
	int result = fstat(fd, &s);
	if (result == 0)
		*flags = s.st_flags;

	return result;
}
