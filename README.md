
# PRN232 Backend (ASP.NET Core)

## Chức năng chính
- EF Core với Npgsql cho PostgreSQL
- Seed đầy đủ mock data (products, users, orders)
- Auth JWT (register/login), endpoints cho products, orders
- Dockerfile sẵn sàng deploy Render

## Chạy local
1. Cài đặt PostgreSQL local hoặc tạo Neon DB (khuyên dùng Neon cho deploy dễ dàng)
2. Sửa `appsettings.json` hoặc đặt biến môi trường:
	- `ConnectionStrings__DefaultConnection` (ví dụ Neon):
	  ```
	  Host=ep-xxx-db.eu-central-1.aws.neon.tech;Database=neondb;Username=neonuser;Password=yourpassword;Port=5432;SSL Mode=Require;Trust Server Certificate=true
	  ```
3. Build và chạy:
	```powershell
	dotnet restore
	dotnet build
	dotnet run
	```

## Migration (tạo bảng, cập nhật schema)
1. Cài dotnet-ef nếu chưa có:
	```powershell
	dotnet tool install --global dotnet-ef
	```
2. Tạo migration đầu tiên:
	```powershell
	dotnet ef migrations add Initial
	dotnet ef database update
	```

## Deploy lên Render
1. Đăng ký tài khoản Render, tạo Web Service mới, kết nối repo này.
2. Chọn deploy bằng Dockerfile (Render tự build từ Dockerfile).
3. Đặt biến môi trường `ConnectionStrings__DefaultConnection` với chuỗi kết nối Neon (như trên).
4. (Khuyên dùng) Đặt thêm biến môi trường `ASPNETCORE_ENVIRONMENT=Production`.
5. Deploy và kiểm tra endpoint `/api/products` hoặc `/swagger`.

## Ghi chú
- Seed data sẽ tự động insert nếu DB trống khi khởi động.
- Có thể mở rộng thêm: xác thực nâng cao, quản lý user, cart, CI/CD tự động migrate DB khi deploy.

## Liên hệ
Nếu cần hỗ trợ thêm, liên hệ nhóm phát triển hoặc xem tài liệu trong repo FE.
