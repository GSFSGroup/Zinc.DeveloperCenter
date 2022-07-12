import { BrowserModule } from "@angular/platform-browser";
import { NgModule } from "@angular/core";

import { MarkdownModule } from "ngx-markdown";
import { HttpClient, HttpClientModule } from "@angular/common/http";
import { AdrDisplayComponent } from "./adr-display.component";

@NgModule({
  declarations: [AdrDisplayComponent],
  imports: [
    BrowserModule,
    HttpClientModule,
    MarkdownModule.forRoot({ loader: HttpClient })
  ],
  providers: [],
  bootstrap: [AdrDisplayComponent]
})
export class AdrDisplayModule {}