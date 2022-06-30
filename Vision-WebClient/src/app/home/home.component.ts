import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }

  openLinkInNewTab(url: string){
    window.open(url, "_blank");
  }

  openLink(url: string){
    window.open(url);
  }
}
