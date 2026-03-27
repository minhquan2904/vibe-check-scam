---
description: Database Naming Convention
---

# Database Naming Convention Rule

## Quy chuẩn chung

### 1. Quy tắc đặt tên

#### 1.2 Quy tắc đặt tên cột

1.  Tính từ + danh từ: Vd dưới DB là `SHORT_NAME`. K đặt `NAME_SHORT`.
2.  Các field có thông tin ngôn ngữ sẽ là `ngôn ngữ` + `tên`: Vd dưới DB Oracle là `VI_NAME`, `EN_NAME`. TUYỆT ĐỐI KHÔNG đặt `NAME_VI`, `NAME_EN` hay chứa thêm tên bảng như `BANK_NAME_VI`.
3.  Quy chuẩn chỉ dùng hậu tố `URL` cho các field lưu đường dẫn: vd `LINK_LOGO` => `LOGO_URL`.
4.  Với tên field được cấu thành từ 2 danh từ thì từ cuối phải đại diện cho đặc tính của field đó. Vd: Key bảo mật server: `serverPrivateKey`, không đặt `privateKeyServer`.
5.  Với tên field có chứa động từ => đưa động từ về thì quá khứ. Vd: ngày nhận: `receivedDate`, không đặt `dateReceive` hoặc `receiveDate`.
6.  Với tên field có chứa số lượng => `NUMBER_OF_*`
7.  Với tên field có chứa giới hạn (ví dụ số lượng icon tối đa) => `LIMIT_OF_ICON`
8.  Các cờ true/false => `IS_*`
9.  Không chứa Prefix tên bảng (hoặc từ đại diện cho bảng): Vd: Cột `CODE` trong bảng `AD_CONFIG` KHÔNG đặt `AD_CONFIG_CODE`. Tương tự với bảng `BO_BENEFICIARY_BANK`, cột mã chỉ là `CODE`, KHÔNG dùng `BANK_CODE`.
10. Ưu tiên dùng `IS_ACITVE` thay cho `STATUS`. Chỉ thêm `STATUS` khi có nhiều hơn 2 trạng thái.
11. Không comment độ dài cột trong DB, chỉ comment khi có yêu cầu.
12. Hạn chế sử dụng `CLOB` hết mức có thể, nếu muốn sử dụng phải hỏi ý kiến.
13. Độ dài tối đa của một cột kiểu `VARCHAR2` là `255`. Chỉ tăng khi tài liệu yêu cầu. Nếu muốn tự tăng thì phải hỏi ý kiến.
