#include <SPI.h>
#include <RFID.h>
#include <Servo.h>
#include <LiquidCrystal_I2C.h>
#include <Wire.h>

#define RESET_PIN 9
#define SDA_PIN 10

#define LedKirmizi 5
#define LedYesil 6
#define Buzzer 7
#define ServoDelay 2000
#define BuzzerDelay1 300
#define BuzzerDelay2 2000
#define ServoPin 3
RFID Rfid(SDA_PIN, RESET_PIN);
Servo servo;
LiquidCrystal_I2C LCD(0x27, 16, 2);
bool AnaEkranDurum = true;
bool KartOkutulduMu = false;
bool Sistem = false;
void setup() {
  pinMode(LedYesil, OUTPUT);
  pinMode(LedKirmizi, OUTPUT);
  pinMode(Buzzer, OUTPUT);
  Serial.begin(9600);
  LCD.begin();
  SPI.begin();
  servo.attach(ServoPin);
  servo.write(0);
  Rfid.init();
}
void loop() {
  AnaEkran();
  if (Rfid.isCard() && Sistem == true) {
    Rfid.readCardSerial();
    String id;
    id += "_";
    id += String(Rfid.serNum[0]);
    id += String(Rfid.serNum[1]);
    id += String(Rfid.serNum[2]);
    id += String(Rfid.serNum[3]);
    id += "_";
    if (id.length() > 11) {
      Serial.println(id);
      KartOkutuldu();
      AnaEkranDurum = false;
      delay(300);
    }
  }
  if (Serial.available() > 0) {
    char str = Serial.read();
    if (str == '7') {
      Sistem = true;
      AnaEkranDurum = true;
    }
    else if (str == '8') {
      Sistem = false;
    }
    else{
      DurumYazdir(str);
      AnaEkranDurum = true;
    }
  }
}
void AnaEkran() {
  if (Sistem == false) {
    LCD.clear();
    LCD.setCursor(0, 0);
    LCD.print(" SISTEM KAPALI! ");
    delay(100);
  }
  else {
    if (AnaEkranDurum) {
      LCD.clear();
      LCD.setCursor(0, 0);
      LCD.print("  LUTFEN KART ");
      LCD.setCursor(0, 1);
      LCD.print("   OKUTUNUZ!  ");
      delay(100);
      AnaEkranDurum = false;
    }
  }
}
void KartOkutuldu() {
  LCD.clear();
  LCD.setCursor(0, 0);
  LCD.print("KART OKUTULDU!");
  LCD.setCursor(0, 1);
  LCD.print(" BEKLEYINIZ.. ");
  delay(100);
}
void DurumYazdir(char durum) {
  AnaEkranDurum = true;
  LCD.clear();
  LCD.setCursor(0, 0);
  if (durum == '1') {
    LCD.print(" TANIMSIZ KART! ");
    Basarisiz();
  }
  else if (durum == '2') {
    LCD.print(" GIRIS BASARILI ");
    Basarili();
  }
  else if (durum == '3') {
    LCD.print("CIKIS YAPILAMAZ");
    Basarisiz();
  }
  else if (durum == '3') {
    LCD.print("CIKIS YAPILAMAZ ");
    Basarisiz();
  }
  else if (durum == '4') {
    LCD.print("GIRIS YAPILAMAZ ");
    Basarisiz();
  }
  else if (durum == '5') {
    LCD.print(" CIKIS BASARILI ");
    Basarili();
  }
  else if (durum == '6') {
    LCD.print("GIRIS VEYA CIKIS");
    LCD.setCursor(0, 1);
    LCD.print("   YAPILAMAZ!!  ");
    Basarisiz();
  }
}
void Basarili() {
  digitalWrite(LedYesil, 1);
  delay(100);
  servo.write(90);
  for (int i = 0; i < 2; i++) {
    digitalWrite(Buzzer, 1);
    delay(BuzzerDelay1);
    digitalWrite(Buzzer, 0);
    delay(BuzzerDelay1);
  }
  delay(ServoDelay);
  servo.write(0);
  digitalWrite(LedYesil, 0);
}
void Basarisiz() {
  digitalWrite(LedKirmizi, 1);
  digitalWrite(Buzzer, 1);
  delay(BuzzerDelay2);
  digitalWrite(LedKirmizi, 0);
  digitalWrite(Buzzer, 0);
  delay(BuzzerDelay2);
}