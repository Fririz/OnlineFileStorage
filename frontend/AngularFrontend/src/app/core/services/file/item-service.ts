import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, firstValueFrom } from 'rxjs'; 
import { environment } from '../../../../environments/environment';
import { Item } from '../../models/Item.model';
import { ItemCreateRequest } from '../../models/item-create-request';
import { BehaviorSubject } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { TypeOfItem } from '../../models/enums/TypeOfItem.enum';
import { HttpHeaders } from '@angular/common/http';
import { of } from 'rxjs';
@Injectable({
  providedIn: 'root',
})
export class ItemService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  public currentFolderId$ = new BehaviorSubject<string | null>(null);
  
  public refresh$ = new BehaviorSubject<void>(undefined);

  public refresh() {
    const currentId = this.currentFolderId$.value;
    this.currentFolderId$.next(currentId);
  }

  getCurrentFolderFiles(): Observable<Item[]> {
    return this.currentFolderId$.pipe(
      switchMap(folderId => {
        if (folderId) {
          return this.getChildren(folderId);
        } else {
          return this.getItemFromRoot();
        }
      })
    );
  }
  goUp() {
    const currentId = this.currentFolderId$.value;
    if (!currentId) return;

    this.getParentFolder(currentId).subscribe({
      next: (parent) => this.setCurrentFolder(parent?.id || null),
      error: () => this.setCurrentFolder(null)
    });
  }

  setCurrentFolder(folderId: string | null) {
    this.currentFolderId$.next(folderId);
    this.refresh();
  }

async uploadFileFlow(file: File): Promise<void> {
    const parentId = this.currentFolderId$.value || null;
    const request: ItemCreateRequest = {
      Name: file.name,
      Type: TypeOfItem.File,
      ParentId: parentId
    };
    const uploadLink = await this.uploadFileGetLink(request);


    await this.uploadFileToCloud(uploadLink, file);

    this.refresh();
  }

  private async uploadFileGetLink(request: ItemCreateRequest): Promise<string> {
const response = await firstValueFrom(
      this.http.post<{ uploadUrl: string }>(`${this.apiUrl}/files`, request)
    );
    return response.uploadUrl;
  }
  private async uploadFileToCloud(url: string, file: File): Promise<void> {
    
    await firstValueFrom(
      this.http.put(url, file, {
        headers: new HttpHeaders({
          'Content-Type': file.type || 'application/octet-stream' 
        })
      })
    );
  }

  getItemFromRoot(): Observable<Item[]> {
    return this.http.get<Item[]>(`${this.apiUrl}/items`);
  }

searchItems(query: string): Observable<Item[]> {
  if (!query || !query.trim()) {
    return of([]); 
  }
  
  return this.http.get<Item[]>(`${this.apiUrl}/items`, {
    params: { search: query }
  });
}

  getParentFolder(itemId: string): Observable<Item> {
    return this.http.get<Item>(`${this.apiUrl}/items/${itemId}/parent`);
  }
  

  async uploadFile(request: ItemCreateRequest): Promise<string> {
    const response = await firstValueFrom(
      this.http.post<{ url: string }>(`${this.apiUrl}/files/`, request)
    );
    this.refresh();
    return response.url; 
  }

  public async getDownloadUrl(fileId: string): Promise<string> {
    const response = await firstValueFrom(
      this.http.get<{ downloadUrl: string }>(`${this.apiUrl}/files/${fileId}`)
    );
    console.log('Download URL:', response.downloadUrl);
    this.refresh();
    
    return response.downloadUrl;
  }

  getChildren(folderId: string): Observable<Item[]> {
    return this.http.get<Item[]>(`${this.apiUrl}/folders/${folderId}/items`);
  }

  async deleteFile(fileId: string): Promise<void> {
    await firstValueFrom(
      this.http.delete<void>(`${this.apiUrl}/files/${fileId}`)
    );
    this.refresh();
  }

  async createFolder(folder: ItemCreateRequest): Promise<void> {
    await firstValueFrom(
      this.http.post<void>(`${this.apiUrl}/folders`, folder)
    );
    this.refresh();
  }

  async deleteFolder(folderId: string): Promise<void> {
    await firstValueFrom(
      this.http.delete<void>(`${this.apiUrl}/folders/${folderId}`)
    );
    this.refresh();
  }
}