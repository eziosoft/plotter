void wyslijInt(int a)
{
  Serial.print(((a >> 0) & 0xFF),BYTE);
  Serial.print(((a >> 8) & 0xFF),BYTE);
}
////////////////////////////////////////////////////////////////////////////////////
int odbierzInt()
{
  byte lowByte = Serial.read();
  byte highByte = Serial.read();
  return ((lowByte << 0) & 0xFF) + ((highByte << 8) & 0xFF00);
}


///////////////////////////////////////////////////////////////////////////////////////

void GoTo( byte pen, int sp, int x1, int y1) //pen, speed, x,y
{
  digitalWrite(led,HIGH);

  int x0=m1;
  int y0=m2;


  int dx = x1 - x0;
  int dy = y1 - y0;

  SetPosition(pen, sp, x0, y0);
  if (abs(dx) > abs(dy))
  {
    float m = (float)dy / (float)dx;      // compute slope
    float b = y0 - m * x0;
    dx = (dx < 0) ? -1 : 1;
    while (x0 != x1)
    {
      x0 += dx;
      SetPosition(pen, sp,x0, (m * x0 + b));

    }
  }
  else
    if (dy != 0)
    {                              // slope >= 1
      float m = (float)dx / (float)dy;      // compute slope
      float b = x0 - m * y0;
      dy = (dy < 0) ? -1 : 1;
      while (y0 != y1)
      {
        y0 += dy;
        SetPosition(pen, sp,(m * y0 + b), y0);
      }
    }


  //potwirdzenie wykonania
  Serial.print(0xFB,BYTE);
  Serial.print(pen, BYTE);
  wyslijInt(sp);
  wyslijInt(m1);
  wyslijInt(m2);

  digitalWrite(led,LOW);
}

///////////////////////////////////////////////////////////////////////////////////////////////////
int t1=0;
byte oldPen=0;
void SetPosition( byte pen, int sp, int x, int y)
{
  if(oldPen!=pen)
  {
    servo.write(pen);
    delay(100);
    oldPen=pen;
  }


  motor1.setSpeed(sp);
  motor2.setSpeed(sp);


  while(m1!=x ||  m2!=y)
  {

    if (m1!=x && m2!=y)
    {
      motor1.setSpeed(sp*2);
      motor2.setSpeed(sp*2);
    }

    if(m1<x)
    {
      m1++;
      motor1.step(1, FORWARD, SINGLE); 
    }
    if(m1>x)
    {
      m1--;
      motor1.step(1, BACKWARD, SINGLE); 
    }
    /////////
    if(m2<y)
    {
      m2++;
      motor2.step(1, FORWARD, SINGLE); 
    }
    if(m2>y)
    {
      m2--;
      motor2.step(1, BACKWARD, SINGLE); 
    }



    //wysylanie statusu
    if(t1==100)
    { 
      Serial.print(0xFA,BYTE);
      Serial.print(pen, BYTE);
      wyslijInt(sp);
      wyslijInt(m1);
      wyslijInt(m2);
      t1=0;
    }
    t1++;
  }

}



int odleglosc(int x1, int y1, int x2, int y2)
{
  return (sqrt(abs(x1-x2)*abs(x1-x2)+abs(y1-y2)*abs(y1-y2)));
}




