---
description: Run security compliance scan using the security-specialist agent on .NET and Angular code.
---

# /security-scan - Security Compliance Scanner Pipeline

$ARGUMENTS

---

## Task

This workflow orchestrates the process of scanning the codebase (.NET backend and Angular frontend) for security vulnerabilities based on a user-provided OWASP or security rule checklist.

**⚠️ CRITICAL RULE:** This workflow is READ-ONLY. It MUST NOT modify any existing source code. Its sole purpose is security auditing and reporting compliance violations.

### Steps:

0. **Checklist Extraction & Approval (MANDATORY GATE)**
   - Đọc `$ARGUMENTS` để nhận dạng đường dẫn file Excel chứa security rules nếu user có truyền vào.
   - Nếu không có `$ARGUMENTS`, sử dụng tool (ví dụ: `Glob` hoặc `Find`) để tự động tìm kiếm các file Excel có thể chứa security rules (`*.xlsx`) trong thư mục hiện tại.
   - Khi tìm thấy các file, hãy chọn một file phù hợp nhất làm **recommendation** (ví dụ: file không chứa chữ "copy"). Hiển thị danh sách file kèm recommendation đó cho user.
     - Nếu user trả lời "tiếp tục" hoặc đồng ý, sử dụng file được recommend.
     - Nếu user chọn file khác trong danh sách hoặc cung cấp đường dẫn mới, sử dụng file user báo.
   - Nếu không tìm thấy file `.xlsx` nào, **bắt buộc** yêu cầu user cung cấp đường dẫn. Nếu user vẫn không cung cấp, hãy DỪNG workflow và thông báo "Không đủ context để tiếp tục thực thi".
   - Run `extract-owasp-rules.py` script để trích xuất rule từ file Excel thành file `owasp_checklist.md`.
   - **TẠM DỪNG (Approval Gate):** Trình bày summary rule đã trích xuất cho User và đợi User confirm hợp lệ **TRƯỚC KHI** bắt đầu scan.

1. **Security Scan Execution (After Approval)**
   - **Agent:** `security-specialist`
   - **Input:** File `owasp_checklist.md` đã được duyệt và mã nguồn .NET, Angular trong workspace.
   - **Task:** Quét mã nguồn một cách có hệ thống theo từng rule trong checklist, phân tích source code và phân loại issues.
   - **Constraint:** READ-ONLY — KHÔNG được sửa source code.
   - **Custom Exclusions:** Tự động bỏ qua (bỏ qua check hoặc đánh dấu là PASS/IGNORED) các rule check liên quan đến **Permissive CORS Configuration** và **HTTPs** vì đây là dự án triển khai nội bộ.

2. **Report Generation & Synthesis**
   - **Agent:** `security-specialist`
   - **Task:** Tổng hợp kết quả scan, tạo bảng thống kê, phân loại mức độ nghiêm trọng (🔴 CRITICAL, 🟠 WARNING, v.v.), vẽ Mermaid Pie Chart và chi tiết lỗi (req_id, file, dòng lệnh).
   - **Output:** Nếu chạy trong Pipeline, lưu vào `openspec/changes/<name>/security-report.md`. Nếu chạy Standalone, hiển thị trực tiếp report lên chat.

---

## Usage Examples

```
/security-scan
/security-scan "OPSWAT_RÀ SOÁT TUÂN THỦ.xlsx"
```

## Before Starting

You MUST read `.agent/agents/security-specialist.md` to strictly follow all 7 internal phases of the `security-specialist` agent during execution.
