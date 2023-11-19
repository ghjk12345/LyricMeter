import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpResponse } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';

@Injectable()
export class MockLyricsInterceptor implements HttpInterceptor {

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (request.url.includes('lyrics.ovh')) {
      const wordCount = this.getRandomInt(100, 400);
      const randomWords = this.generateRandomWords(wordCount);

      const delayMs = this.getRandomInt(90, 450);

      const httpResponse = new HttpResponse({
        status: 200,
        body: { lyrics: randomWords }
      });

      return of(httpResponse).pipe(delay(delayMs));
    }

    return next.handle(request);
  }

  private getRandomInt(min: number, max: number): number {
    return Math.floor(Math.random() * (max - min + 1)) + min;
  }

  private generateRandomWords(count: number): string {
    const words = ['lorem', 'ipsum', 'dolor', 'sit', 'amet', 'consectetur', 'adipiscing', 'elit'];
    let lyrics = '';
    for (let i = 0; i < count; i++) {
      lyrics += words[this.getRandomInt(0, words.length - 1)] + ' ';
    }
    return lyrics.trim();
  }
}
