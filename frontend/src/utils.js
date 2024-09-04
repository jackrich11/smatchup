export const getEnvVar = function(envVar) {    

    if(process.env.BUILD_ENV === undefined) {
        return import.meta.env["VITE_" + envVar]
    } else if(process.env.BUILD_ENV === "dev") {
        envVar = "DEV_" + envVar
    } else if(process.env.BUILD_ENV === "prod") {
        envVar = "PROD_" + envVar
    } else {
        console.log("ERROR: Couldn't read BUILD_ENV environment variable.")
        return null
    }
    return process.env[envVar]
}