#define _GNU_SOURCE
#include <stdint.h>
#include <linux/stat.h>
#include <sys/syscall.h>
#include <unistd.h>

struct neme_statx_timestamp
{
	int64_t seconds;
	uint32_t nanoseconds;
	int32_t reserved;
};

struct neme_statx
{
	uint32_t mask;
	uint32_t block_size;
	uint64_t attributes;
	uint32_t link_count;
	uint32_t uid;
	uint32_t gid;
	uint16_t mode;
	uint16_t reserved0;
	uint64_t inode;
	uint64_t size;
	uint64_t blocks;
	uint64_t attributes_mask;
	struct neme_statx_timestamp access_time;
	struct neme_statx_timestamp creation_time;
	struct neme_statx_timestamp change_time;
	struct neme_statx_timestamp write_time;
	uint32_t rdev_major;
	uint32_t rdev_minor;
	uint32_t dev_major;
	uint32_t dev_minor;
	uint64_t mount_id;
	uint32_t dio_mem_align;
	uint32_t dio_offset_align;
	uint64_t reserved1[12];
};

int neme_statx(int dirfd, const char* pathname, int flags, unsigned int mask, struct neme_statx* result)
{
	struct statx statx_result;
	int rv = syscall(SYS_statx, dirfd, pathname, flags, mask, &statx_result);
	if (rv == 0)
	{
		result->mask = statx_result.stx_mask;
		result->block_size = statx_result.stx_blksize;
		result->attributes = statx_result.stx_attributes;
		result->link_count = statx_result.stx_nlink;
		result->uid = statx_result.stx_uid;
		result->gid = statx_result.stx_gid;
		result->mode = statx_result.stx_mode;
		result->reserved0 = 0;
		result->inode = statx_result.stx_ino;
		result->size = statx_result.stx_size;
		result->blocks = statx_result.stx_blocks;
		result->attributes_mask = statx_result.stx_attributes_mask;
		result->access_time.seconds = statx_result.stx_atime.tv_sec;
		result->access_time.nanoseconds = statx_result.stx_atime.tv_nsec;
		result->access_time.reserved = statx_result.stx_atime.__reserved;
		result->creation_time.seconds = statx_result.stx_btime.tv_sec;
		result->creation_time.nanoseconds = statx_result.stx_btime.tv_nsec;
		result->creation_time.reserved = statx_result.stx_btime.__reserved;
		result->change_time.seconds = statx_result.stx_ctime.tv_sec;
		result->change_time.nanoseconds = statx_result.stx_ctime.tv_nsec;
		result->change_time.reserved = statx_result.stx_ctime.__reserved;
		result->write_time.seconds = statx_result.stx_mtime.tv_sec;
		result->write_time.nanoseconds = statx_result.stx_mtime.tv_nsec;
		result->write_time.reserved = statx_result.stx_mtime.__reserved;
		result->rdev_major = statx_result.stx_rdev_major;
		result->rdev_minor = statx_result.stx_rdev_minor;
		result->dev_major = statx_result.stx_dev_major;
		result->dev_minor = statx_result.stx_dev_minor;
		result->mount_id = statx_result.stx_mnt_id;
		result->dio_mem_align = statx_result.stx_dio_mem_align;
		result->dio_offset_align = statx_result.stx_dio_offset_align;

		for (int i = 0; i < 12; ++i)
			result->reserved1[i] = statx_result.__spare3[i];
	}

	return rv;
}
