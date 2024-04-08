# FishSyncServer

- [o] RFiles.NET 으로 IFileChecksumStorage 구현
- [x] Entity 의 EF.core 의존성 제거 -> ApplicationDbContext 로 이동
- [x] LocalFileChecksumStorage 구현
- [x] User: 회원가입 로그인 인증 버킷 권한
- [x] 감사: sync 로그 언제 누가
- [x] Bucket 종류: ChecksumBaseBucket, PathBaseBucket, RemoteBucket

# WEB

웹에서는 버킷 상태 확인, 설정 변경 정도만 가능하고 한 페이지에서 모든 정보를 다 뿌려줌
- GET /buckets?owner={owner-id}
- GET /buckets/{bucket-id}
- GET /buckets/{bucket-id}/storages

- GET /bucket-indexes?q={search-query}&private={true|false}
- GET /bucket-indexes/{index-id}
- PUT /bucket-indexes/{index-id}
- POST /bucket-indexes/{index-id}
- DELETE /bucket-indexes/{index-id}
- POST /bucket-indexes/{index-id}/buckets
- DELETE /bucket-indexes/{index-id}/buckets/{bucket-id}

- GET /caches/checksums
- POST /caches/checksums/purge
- POST /caches/checksums/update

# API

- GET /buckets?owner={owner-id}
- GET /buckets/{bucket-id}
- PUT /buckets/{bucket-id}
- GET /buckets/{bucket-id}/limitations
- GET /buckets/{bucket-id}/files
- GET /buckets/{bucket-id}/storages
- GET /buckets/{bucket-id}/storages/{storage-id}
- POST /buckets/{bucket-id}/sync

- GET /bucket-indexes/{index-id}
- POST /bucket-indexes/{index-id}/buckets
- DELETE /bucket-indexes/{index-id}/buckets/{bucket-id}

- POST /auth/login
- POST /auth/logout