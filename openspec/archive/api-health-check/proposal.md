# Proposal: API Health Check

## 1. Feature Overview

**Feature Name**: `api-health-check`
**Type**: NEWBUILD
**Flow Type**: Query
**Created**: 2026-03-27

### Mục đích

Cung cấp một endpoint HTTP tiêu chuẩn để kiểm tra trạng thái hoạt động (liveness & readiness) của backend service. Endpoint này được gọi bởi:

- Load balancer / reverse proxy (nginx, API Gateway) để định tuyến traffic
- Infrastructure monitoring (Prometheus, Grafana, Uptime Robot, Zabbix)
- CI/CD pipeline để xác nhận deployment thành công
- Kubernetes liveness probe / readiness probe

### Phạm vi

Endpoint đơn giản, **không yêu cầu authentication**, trả về thông tin:
- Trạng thái tổng thể của service (`UP` / `DEGRADED` / `DOWN`)
- Trạng thái từng dependency quan trọng (database, cache, external API…)
- Thông tin phiên bản service (version, build info)
- Uptime / timestamp hiện tại

---

## 2. Business Flow

```
Client (monitor / LB / CD) → GET /health
                                  ↓
                        HealthCheckController
                                  ↓
                        HealthCheckService
                        ├── check DB connection
                        ├── check Redis/Cache ping
                        └── check external API (optional)
                                  ↓
                        Aggregate → HealthStatus response
```

**Không có**: database write, state mutation, authentication middleware.

---

## 3. Affected Services / Modules

| Service / Module | Impact | Lý do |
|-----------------|--------|-------|
| `API Layer` | **ADDED** | Thêm `/health` endpoint mới |
| `Infrastructure Config` | **ADDED** | Exclude route /health khỏi auth pipeline |
| `Database Context` | **READ ONLY** | Ping / connection check |
| `IMemoryCache / Redis` | **READ ONLY** | Ping check (nếu có) |

---

## 4. API Specification (High-Level)

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| GET | `/health` | ❌ None | Liveness — kiểm tra service còn sống |
| GET | `/health/ready` | ❌ None | Readiness — kiểm tra sẵn sàng nhận traffic (có check deps) |

### Response mẫu — `/health`

```json
{
  "status": "UP",
  "timestamp": "2026-03-27T13:30:00+07:00",
  "version": "1.0.0",
  "uptime": 3600
}
```

### Response mẫu — `/health/ready`

```json
{
  "status": "UP",
  "timestamp": "2026-03-27T13:30:00+07:00",
  "version": "1.0.0",
  "uptime": 3600,
  "dependencies": {
    "database": { "status": "UP", "responseTimeMs": 12 },
    "cache":    { "status": "UP", "responseTimeMs": 3 }
  }
}
```

**HTTP Status Codes:**
| Condition | Status Code |
|-----------|-------------|
| Tất cả dependency UP | `200 OK` |
| Một số dependency DOWN nhưng service vẫn chạy | `200 OK` với `status: DEGRADED` |
| Service không khởi động được | `503 Service Unavailable` |

---

## 5. External Integrations

- **Không có** external API mới cần integrate.
- Chỉ sử dụng internal connectivity checks (DB ping, cache ping).

---

## 6. Dependencies & Risks

| Risk | Mức độ | Mitigation |
|------|--------|------------|
| DB check timeout làm chậm response | Thấp | Đặt timeout ngắn (≤ 500ms), trả về `DEGRADED` nếu quá hạn |
| Endpoint bị lộ thông tin stack/version | Thấp | Giới hạn chi tiết trả về theo env (dev vs prod) |
| Endpoint gây tải lên DB khi monitor liên tục | Thấp | Dùng lightweight ping query (`SELECT 1`) |
| Route auth bypass không được cấu hình đúng | Trung bình | Cấu hình exclude route `/health*` trong auth middleware |

---

## 7. Feature Profile (Locked)

```
🔒 Feature Profile:
   Mode:         Query (read-only, no entity changes)
   Flow Type:    Query
   Type:         NEWBUILD
   Auth:         EXCLUDED (public endpoint)
   Layer:        Controller → Service (no Repository needed)
```

---

## 8. High-Level Impact Assessment

- **Rủi ro thấp**: Endpoint read-only, không thay đổi data
- **Không break changes**: Thêm route mới, không sửa route cũ
- **Cần cấu hình**: Exclude `/health` khỏi JWT middleware
- **Tương thích**: Swagger/OpenAPI nên exclude endpoint này (hoặc mark as public)
