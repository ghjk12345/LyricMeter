import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { FormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { MockLyricsInterceptor } from './interceptors/mockLyricsInterceptor';
import { DetailsComponent } from './details/details.component';
import { HighchartsChartModule } from 'highcharts-angular';
import { NotFoundComponent } from './not-found/not-found.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    DetailsComponent,
    NotFoundComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule,
    HighchartsChartModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: MockLyricsInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
