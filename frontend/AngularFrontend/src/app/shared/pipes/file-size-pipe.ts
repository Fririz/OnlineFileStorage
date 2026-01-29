import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'filesize',  standalone: true }) 
export class FileSizePipe implements PipeTransform {

  transform(sizeInBytes: number): string {
    if (sizeInBytes < 1024) return sizeInBytes + ' B';
    
    const k = 1024;
    const sizes = ['B', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(sizeInBytes) / Math.log(k));
    
    return parseFloat((sizeInBytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  }
}