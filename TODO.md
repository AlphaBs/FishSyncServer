- [o] RFiles.NET 으로 IFileChecksumStorage 구현
- [x] Entity 의 EF.core 의존성 제거 -> ApplicationDbContext 로 이동
- [x] LocalFileChecksumStorage 구현
- [x] User: 회원가입 로그인 인증 버킷 권한
- [x] 감사: sync 로그 언제 누가
- [x] Bucket 종류: ChecksumBaseBucket, PathBaseBucket, RemoteBucket

# API

- GET /buckets?owner={owner-id}
- GET /buckets/{bucket-id}
- PUT /buckets/{bucket-id}
- GET /buckets/{bucket-id}/limitations
- GET /buckets/{bucket-id}/files
- GET /buckets/{bucket-id}/storages
- GET /buckets/{bucket-id}/storages/{storage-id}
- POST /buckets/{bucket-id}/sync

- GET /bucket-indexes?q={search-query}&private={true|false}
- GET /bucket-indexes/{index-id}
- PUT /bucket-indexes/{index-id}
- POST /bucket-indexes/{index-id}
- DELETE /bucket-indexes/{index-id}

- GET /caches/checksums
- POST /caches/checksums/purge
- POST /caches/checksums/update

- GET /users
- PUT /users/{user-id}
- POST /users/{user-id}
- DELETE /users/{user-id}

- POST /auth/login
- POST /auth/logout