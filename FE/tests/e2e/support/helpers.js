export function generateRandomString() {
  var text = "";
  var possibleChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

  for (var i = 0; i < 10; i++)
    text += possibleChars.charAt(
      Math.floor(Math.random() * possibleChars.length)
    );

  return text;
}
