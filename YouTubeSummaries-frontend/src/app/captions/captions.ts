import { Component, Input } from '@angular/core';
import { TranscriptResponse } from '../models/TranscriptResponse';

@Component({
  imports: [],
  selector: 'app-captions',
  styleUrl: './captions.css',
  templateUrl: './captions.html',
})
export class Captions {
  @Input() data: TranscriptResponse | undefined;
}
