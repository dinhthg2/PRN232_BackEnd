-- Fix migration history
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion") 
VALUES ('20251015151930_InitialCreate', '8.0.4')
ON CONFLICT ("MigrationId") DO NOTHING;