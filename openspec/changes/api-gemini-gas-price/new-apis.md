# New APIs

### 1. `GET /api/v1/gas-price/today`
- **Purpose**: Fetch current Vietnam gas prices (RON 95, E5 RON 92, Diesel) via Gemini AI integration.
- **Authentication**: `[AllowAnonymous]` (Public Access)
- **Request Parameters**: None
- **Response Structure (200 OK)**:
  ```json
  {
    "price": 24000,
    "unit": "VND/lít",
    "ron95": 24000,
    "e5ron92": 23000,
    "diesel": 20000,
    "timestamp": "2024-03-27T12:00:00Z"
  }
  ```
- **Error Responses**:
  - `503 Service Unavailable`: If the external dependency (Gemini API) times out or is unreachable.
  - `500 Internal Server Error`: For JSON parsing failures or general exceptions.
