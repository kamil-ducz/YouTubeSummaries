export type CaptionLine = {
  text: string;
  startInSeconds: number;
}

export type TranscriptResponse = {
  title: string;
  language: string;
  captions: CaptionLine[];
}