import { ChangeDetectorRef, Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { YouTubeService } from '../services/summary-service';
import { TranscriptResponse } from '../models/TranscriptResponse';
import { Captions } from '../captions/captions';
import { GenerateNotesRequest } from '../models/GenerateNotesRequest';
import { Notes } from '../notes/notes';

@Component({
  imports: [FormsModule, Captions, Notes],
  selector: 'app-url',
  styleUrl: './url.css',
  templateUrl: './url.html',
})
export class Url {

  constructor(private youtubeService: YouTubeService, private cdr: ChangeDetectorRef) {}

  videoUrl! : string;
  transcriptResponse! : TranscriptResponse | undefined;
  requestData! : GenerateNotesRequest | undefined;
  notesResponse! : string;

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
        
        console.log(`Connected with backend!\nFilm: ${response.title}`);
      },
      error: (err) => {
        console.error('Did not connect with the backend:', err);
      }
    });
  }

  onGenerateNotes() : void {
    if (!this.transcriptResponse) {
      alert('Please download the transcript first.');
      return;
    }

    const requestData = {
      transcript: this.transcriptResponse.captions.map(caption => caption.text).join(' '),
      language: 'en', // Default language
      length: 'medium', // Default length
      style: 'informative' // Default style
    } as GenerateNotesRequest;

    this.requestData = requestData;

    this.youtubeService.generateNotes(requestData).subscribe({
      next: (response) => {
        this.notesResponse = response.notes;
        this.cdr.detectChanges();
        console.log('Notes generated successfully' + response);
      },
      error: (err) => {
        console.error('Error generating notes:', err);
      }
    });
  }
}