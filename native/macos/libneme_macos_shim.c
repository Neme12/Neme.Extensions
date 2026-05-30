#include <fcntl.h>

int neme_fcntl_getpath(int fd, char* path)
{
	return fcntl(fd, F_GETPATH, path);
}
