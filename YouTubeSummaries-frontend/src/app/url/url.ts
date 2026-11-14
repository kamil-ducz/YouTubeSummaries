import { ChangeDetectorRef, Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { YouTubeService } from '../services/summary-service';
import { TranscriptResponse } from '../models/TranscriptResponse';
import { Captions } from '../captions/captions';

@Component({
  imports: [FormsModule, Captions],
  selector: 'app-url',
  styleUrl: './url.css',
  templateUrl: './url.html',
})
export class Url {

  constructor(private youtubeService: YouTubeService, private cdr: ChangeDetectorRef) {}

  videoUrl! : string;
  transcriptResponse! : TranscriptResponse | undefined;

  onDownloadTranscript() : void {
    if (!this.videoUrl) {
      alert('Please enter a YouTube video URL.');
      return;
    }

    console.log("Downloading transcript for video URL:", this.videoUrl);

    this.youtubeService.getTranscript(this.videoUrl).subscribe({
      next: (response) => {
        this.transcriptResponse = response;
        this.cdr.detectChanges();
        console.log('Success! Film title:', response.title);
        console.log('Transcription lines count:', response.captions.length);
        console.log('Full backend object:', response);
        
        alert(`Connected with backend!\nFilm: ${response.title}`);
      },
      error: (err) => {
        console.error('Did not connect with the backend:', err);
        alert('Did not download the transcription. Check console (F12) and backend logs.');
      }
    });
  }
}