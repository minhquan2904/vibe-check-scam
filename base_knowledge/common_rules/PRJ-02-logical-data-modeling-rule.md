---
description: Logical Data Modeling Rules - Quy tắc thiết kế bảng dữ liệu
---
# Logical Data Modeling Rules

Tài liệu này định nghĩa các quy chuẩn BẮT BUỘC khi thiết kế Mô hình Dữ liệu Logic (Logical Data Model) cho khối Backoffice ngân hàng.

## 1. QUY TẮC CỐT LÕI (REUSE PRECEDES INVENTION)
- TRƯỚC KHI THIẾT KẾ BẢNG MỚI: Phải tìm xem cấu trúc tương tự đã có chưa. Nếu có, kế thừa 100% thay vì tự vẽ vời không gian mới.
- Không tự thêm bảng HISTORY (lưu vết thay đổi) nếu requirement không bắt buộc.

## 2. Audit Fields Bắt Buộc
MỌI BẢNG ĐỀU PHẢI CÓ các trường kiểm toán sau (không có ngoại lệ):
- `CREATED_DATE`: Bắt buộc kiểu `TIMESTAMP` hoặc `DATE`.
- `CREATED_BY`: Bắt buộc kiểu `NUMBER` (User ID hệ thống ngân hàng luôn là NUMBER).
- `MODIFIED_DATE`: Bắt buộc kiểu `TIMESTAMP` hoặc `DATE`.
- `MODIFIED_BY`: Bắt buộc kiểu `NUMBER`.
- `IS_ACTIVE`: Bắt buộc kiểu `NUMBER`.

## 3. Khóa chính & Sequence
Quyết định loại khóa chính dựa trên Requirement:
- Nếu PK là **VARCHAR2 (CODE/Mã Dịnh Danh)**: KHÔNG tạo SEQUENCE.
- Nếu PK là **NUMBER (ID tự tăng)**: BẮT BUỘC ghi chú vào Handoff là phải tạo SEQUENCE theo chuẩn `<SCHEMA>.<TABLE_NAME>_SEQ` và gán default là `NEXTVAL`.
