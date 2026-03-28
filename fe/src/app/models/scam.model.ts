export interface TransactionItem {
  rawContent: string;
}

export interface ScamDetectionResult {
  isScam: boolean;
  confidenceScore: number;
  reason: string;
}

export interface CheckScamRequest {
  rawText: string;
}

export interface CheckScamResponse {
  transactions: TransactionItem[];
  scamAnalysis: ScamDetectionResult;
}
