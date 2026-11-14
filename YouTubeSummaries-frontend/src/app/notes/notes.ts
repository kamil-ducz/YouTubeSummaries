import { Component, Input, ViewChild, ElementRef } from '@angular/core';
import html2pdf from 'html2pdf.js';

@Component({
  imports: [],
  selector: 'app-notes',
  styleUrl: './notes.css',
  templateUrl: './notes.html',
})
export class Notes {
    @Input() data!: string;

    @ViewChild('notesContainer') notesContainer!: ElementRef;

    downloadNotesFile(): void {
      if (!this.data || !this.notesContainer) return;

      const element = this.notesContainer.nativeElement;

      const options = {
        margin: 15,
        filename: 'notes.pdf',
        image: { type: 'jpeg', quality: 0.98 },
        html2canvas: { scale: 2 },
        jsPDF: { unit: 'mm', format: 'a4', orientation: 'portrait' }
      } as const;

      html2pdf().set(options).from(element).save();
    }

}
