# NetworkTimeChecker
This is a simple tool for getting the date and time from the internet, for use in Unity as an alternative to checking the system time. I made this because I wanted to create a time-based game mechanic which couldn't be tampered with by changing the PC's clock.

I use multithreading and a callback pattern so you don't have to stall your game while waiting for the response.

That said, there's pretty much no security on the web request. This just makes things marginally harder.

## How to Use

You just call `NetworkTimeChecker.RequestTime`. The method takes the timeout duration in seconds, so you can customize how long until the request is abandoned. The result is sent to the callback provided. The time is provided in [Unix Epoch time](https://en.wikipedia.org/wiki/Unix_time) and sourced from the NIST Internet Time Service.

![image](https://user-images.githubusercontent.com/18707147/173845985-310586d8-e245-4c9a-bd22-4cf70ad5b30e.png)

![image](https://user-images.githubusercontent.com/18707147/173847211-d91a34fd-4c79-4950-9721-1fc755684995.png)

![image](https://user-images.githubusercontent.com/18707147/173847259-0ba2be83-ac2b-4c70-bdc9-9cd0028906f6.png)

