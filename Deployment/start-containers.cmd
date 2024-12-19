cd ..
echo 'Building API image'
call docker build --no-cache -t sis_api .
cd Deployment
echo 'Starting containers'
call docker compose -p sis_projekt up -d