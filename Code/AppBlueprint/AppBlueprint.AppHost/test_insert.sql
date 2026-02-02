INSERT INTO "FileMetadata" ("Id", "FileKey", "OriginalFileName", "ContentType", "SizeInBytes", "UploadedBy", "Folder", "IsPublic", "PublicUrl", "CustomMetadata", "TenantId", "CreatedAt", "IsSoftDeleted")
VALUES ('test_123', 'test/key.txt', 'test.txt', 'text/plain', 100, 'user1', 'test', false, NULL, '{}', 'tenant_test', NOW(), false);
