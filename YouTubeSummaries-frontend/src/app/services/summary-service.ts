import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { GenerateNotesRequest } from "../models/GenerateNotesRequest";
import { TranscriptResponse } from "../models/TranscriptResponse";

@Injectable({
    providedIn: 'root'
})

export class YouTubeService {
    private url = 'https://youtubesummaries-backend-fbh5b6gva9hhbmap.westus3-01.azurewebsites.net/api/YouTubeSummaries';

    constructor(private http: HttpClient) { }

    getTranscript(videoUrl: string): Observable<TranscriptResponse> {
        return this.http.get<TranscriptResponse>(`${this.url}`, {
            params: { videoUrl: videoUrl }
        })
    }

    generateNotes(requestData: GenerateNotesRequest): Observable<{ notes: string }> {
    return this.http.post<{ notes: string }>(`${this.url}`, requestData);
  }    
}