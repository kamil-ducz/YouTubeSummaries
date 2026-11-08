export interface CaptionLine {
  text: string;
  startInSeconds: number;
}

export interface TranscriptResponse {
  title: string;
  language: string;
  captions: CaptionLine[];
}