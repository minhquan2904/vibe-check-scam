---
name: confluence-reader
description: Read Confluence pages recursively — detect parent/child hierarchy, discover linked pages, and ingest all related content into a single structured output.
---

## Purpose

Nhận một URL/page_id Confluence, tự động:
1. Xác định page đó là **parent** (có child pages) hay **leaf** (không có con)
2. Nếu là parent → đệ quy đọc tất cả child pages
3. Scan nội dung tìm **link Confluence khác** được nhúng (cross-references)
4. Đọc tất cả linked pages phát hiện được
5. Trả về **toàn bộ nội dung đã gom** dạng structured markdown

**Output**: Nội dung text đầy đủ của page + tất cả child + linked pages, sẵn sàng để workflow khác sử dụng (VD: `wf-pre-openspec`, `feat-propose`).

---

## Input

| Param | Required | Description |
|---|---|---|
| `CONFLUENCE_URL` | Có (1 trong 2) | Full URL dạng `https://{domain}/wiki/spaces/{SPACE}/pages/{PAGE_ID}/...` |
| `PAGE_ID` | Có (1 trong 2) | Page ID trực tiếp (VD: `123456789`) |
| `MAX_DEPTH` | Không | Độ sâu đệ quy child pages. Default: `3` |
| `FOLLOW_LINKS` | Không | Có đọc các page Confluence được link trong nội dung không? Default: `true` |
| `OUTPUT_VAR` | Không | Tên biến trả về. Default: `CONFLUENCE_CONTENT` |

---

## Step 1 — Parse Input & Xác định Page ID

### 1a. Nếu input là URL → extract Page ID
Confluence URL patterns:
```
https://{domain}/wiki/spaces/{SPACE_KEY}/pages/{PAGE_ID}/{PAGE_TITLE}
https://{domain}/wiki/spaces/{SPACE_KEY}/pages/{PAGE_ID}
https://{domain}/wiki/x/{SHORT_ID}
```

Extract `PAGE_ID` từ URL bằng regex:
- Pattern: `/pages/(\d+)`
- Nếu không match → thử extract `space_key` + `title` từ URL

### 1b. Gọi MCP tool đọc page gốc

```
mcp_atlassian_confluence_get_page(
    page_id = "{PAGE_ID}",
    include_metadata = true,
    convert_to_markdown = true
)
```

**Lưu kết quả**:
- `ROOT_PAGE`: nội dung + metadata
- `ROOT_TITLE`: tiêu đề page
- `ROOT_SPACE_KEY`: space key

---

## Step 2 — Xác định Parent hay Leaf

### 2a. Kiểm tra có child pages không

```
mcp_atlassian_confluence_get_page_children(
    parent_id = "{PAGE_ID}",
    limit = 50,
    include_content = false
)
```

### 2b. Phân loại

| Kết quả | Loại | Hành động tiếp |
|---|---|---|
| Có children (count > 0) | **PARENT** | → Step 3 (đệ quy đọc children) |
| Không có children | **LEAF** | → Step 4 (scan links) |

**Log**: Ghi nhận `[INFO] Page "{ROOT_TITLE}" is {PARENT|LEAF} with {N} children`

---

## Step 3 — Đệ quy đọc Child Pages

### Algorithm

```
function readPageTree(pageId, currentDepth, maxDepth):
    if currentDepth > maxDepth:
        return []
    
    children = get_page_children(pageId)
    results = []
    
    for child in children:
        # Đọc nội dung child
        content = get_page(child.id, convert_to_markdown=true)
        results.append(content)
        
        # Đệ quy nếu child cũng có children
        results += readPageTree(child.id, currentDepth + 1, maxDepth)
    
    return results
```

### Implementation

Cho mỗi child page:

```
mcp_atlassian_confluence_get_page(
    page_id = "{CHILD_ID}",
    include_metadata = true,
    convert_to_markdown = true
)
```

**Tracking**: Duy trì danh sách `VISITED_PAGES` (Set of page_id) để tránh đọc trùng.

---

## Step 4 — Scan Nội Dung Tìm Linked Pages

### 4a. Tìm Confluence links trong nội dung markdown

Sau khi đọc mỗi page, scan nội dung tìm patterns:
```
Patterns to match:
1. [text](https://{domain}/wiki/spaces/{SPACE}/pages/{PAGE_ID}/...)
2. [text](/wiki/spaces/{SPACE}/pages/{PAGE_ID}/...)
3. <ac:link><ri:page ri:content-title="Page Title" ri:space-key="SPACE"/></ac:link>
4. href="/wiki/spaces/{SPACE}/pages/{PAGE_ID}"
```

Extract tất cả `PAGE_ID` từ links → thêm vào `LINKED_PAGES` queue.

### 4b. Đọc linked pages

```
if FOLLOW_LINKS == true:
    for linked_page_id in LINKED_PAGES:
        if linked_page_id NOT IN VISITED_PAGES:
            content = get_page(linked_page_id)
            VISITED_PAGES.add(linked_page_id)
            # KHÔNG đệ quy children của linked pages
            # KHÔNG scan links của linked pages (tránh vòng lặp vô hạn)
```

> ⚠️ Linked pages chỉ đọc nội dung trực tiếp, KHÔNG đệ quy children và KHÔNG follow links tiếp.

---

## Step 5 — Tổng hợp Output

### Output format

Trả về biến `CONFLUENCE_CONTENT` chứa toàn bộ nội dung đã gom, structured:

```markdown
# Confluence Content Report

## Summary
- Root Page: {ROOT_TITLE} (ID: {PAGE_ID})
- Type: {PARENT|LEAF}
- Total pages read: {COUNT}
- Child pages: {CHILD_COUNT}
- Linked pages: {LINKED_COUNT}

---

## 📄 Root: {ROOT_TITLE}
{Root page content in markdown}

---

## 📁 Children

### 📄 {Child 1 Title}
{Child 1 content}

### 📄 {Child 2 Title}
{Child 2 content}

#### 📄 {Grandchild Title}
{Grandchild content — indented by depth}

---

## 🔗 Linked Pages

### 📄 {Linked Page Title}
{Linked page content}
```

### Heading depth
- Root page: `## 📄`
- Child depth 1: `### 📄`
- Child depth 2: `#### 📄`
- Linked pages: `### 📄` (under Linked Pages section)

---

## Step 6 — Trả kết quả

### Nếu gọi standalone
- In ra toàn bộ `CONFLUENCE_CONTENT`
- Lưu vào file nếu có `OUTPUT_FILE` param

### Nếu gọi từ workflow khác
- Set biến `CONFLUENCE_CONTENT` để workflow cha sử dụng
- Return summary: page count, titles list

---

## Error Handling

| Lỗi | Xử lý |
|---|---|
| Page not found (404) | Log warning, skip, continue with other pages |
| Permission denied | Log warning, ghi nhận "Access denied for page {ID}" |
| Rate limit | Wait 2s, retry 1 lần |
| Invalid URL | Return error message với URL pattern hướng dẫn |
| Quá nhiều pages (>50) | Log warning, chỉ đọc 50 pages đầu, ghi nhận còn lại |

---

## Guardrails

1. **Tránh vòng lặp**: LUÔN check `VISITED_PAGES` trước khi đọc
2. **Giới hạn đệ quy**: MAX_DEPTH mặc định = 3. Quá 3 level hiếm khi cần
3. **Linked pages không đệ quy**: Chỉ đọc nội dung, KHÔNG follow children/links tiếp
4. **Giới hạn tổng pages**: Tối đa 50 pages mỗi lần chạy
5. **Confluence MCP tools**: Dùng `mcp_atlassian_confluence_*` tools — KHÔNG dùng browser
6. **Metadata luôn bật**: `include_metadata = true` để lấy last_modified, author
7. **Markdown output**: `convert_to_markdown = true` cho đọc dễ hơn
