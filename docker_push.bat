docker build -t jane_dev:latest -f Jane.Server/Dockerfile .
docker tag jane_dev:latest junglejane/jane_dev:0.0.1
docker tag jane_dev:latest junglejane/jane_dev:latest
docker push junglejane/jane_dev:0.0.1
docker push junglejane/jane_dev:latest
