import { Component, Input } from '@angular/core';

@Component({
  imports: [],
  selector: 'app-notes',
  styleUrl: './notes.css',
  templateUrl: './notes.html',
})
export class Notes {
    @Input() data!: string;

downloadNotesFile() {
throw new Error('Method not implemented.');
}

}
