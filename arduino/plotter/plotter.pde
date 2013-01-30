#include <Servo.h>
#include <AFMotor.h>

AF_Stepper motor1(48, 2);
AF_Stepper motor2(48, 1);
Servo servo;

#define led 13
#define xMax 7800
#define yMax 8800
#define penUp 80

int m1=0;
int m2=0;

void setup()
{
  pinMode(led,OUTPUT);
  Serial.begin(115200);
  motor1.setSpeed(450);
  motor2.setSpeed(450);
  servo.attach(9);
  servo.write(penUp);
}


void loop()
{
  if (Serial.available()>=8)
  {

    if (Serial.read()==0xFA)
    {

      byte pen = Serial.read();
      int sp = odbierzInt();
      int x = odbierzInt();
      int y = odbierzInt();

      if(x<0) x=0;
      if(x>xMax) x=xMax;
      if(y<0) y=0;
      if(y>yMax) y=yMax;



      GoTo(pen,sp,x,y);

    }
  }


}




