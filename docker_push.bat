docker build -t jane_dev:latest -f Jane.Server/Dockerfile .
docker tag jane_dev:latest cysharp/jane_dev:0.0.1
docker tag jane_dev:latest cysharp/jane_dev:latest
docker push cysharp/jane_dev:0.0.1
docker push cysharp/jane_dev:latest
