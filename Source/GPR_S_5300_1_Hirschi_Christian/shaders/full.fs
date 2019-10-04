#version 400 core

uniform sampler2D baseColorMap;
uniform mat4 light;
uniform bool correctSpeclarReflection;

smooth in vec4 p_color;
smooth in vec2 p_texcoords;
smooth in vec3 p_normal;
smooth in vec3 p_position;
smooth in vec3 p_vertexNormalWorldSpace;
out vec4 fragColor;

vec3 ambientReflection(float intensity, float factor, vec3 lightColor) {
	return intensity * factor * lightColor;
}

vec3 diffuseReflection(float intensity, float factor, vec3 lightColor, vec3 lightDirection, vec3 normal) {
	return clamp(dot(lightDirection, normal),0.0f,1.0f) * intensity * factor * lightColor;
}

vec3 phongSpecularReflection(float intensity, float factor, vec3 lightColor, float hardness, vec3 viewDirection, vec3 reflectionDirection) {
	return pow(clamp(dot(viewDirection, reflectionDirection),0.0f,1.0f),hardness) * intensity * factor * lightColor;
}

vec3 blinnSpecularReflection(float intensity, float factor, vec3 lightColor, float hardness, vec3 normal, vec3 halfVector) {
	return pow(clamp(dot(normal, halfVector),0.0f,1.0f),hardness) * intensity * factor * lightColor;
}

void main()
{
   vec3 lightPosition = light[0].xyz;
   vec3 ambientLightColor = light[1].xyz;
   vec3 lightColor = light[2].xyz;
   vec3 viewPosition = light[3].xyz;

   float ambientIntensity = light[0].w;
   float diffuseIntensity = light[1].w;
   float specularIntensity = light[2].w;
   float hardness = light[3].w;

   float ambientFactor = 1.0f;
   float diffuseFactor = 1.0f;
   float specularFactor = 1.0f;

   vec3 lightDirection = normalize(lightPosition - p_position);
   vec3 viewDirection = normalize(viewPosition - p_position);

   vec3 baseColor = texture(baseColorMap, p_texcoords).rgb;
   vec3 ambient = ambientReflection(ambientIntensity, ambientFactor, ambientLightColor);
   vec3 diffuse = diffuseReflection(diffuseIntensity, diffuseFactor, lightColor, lightDirection, p_vertexNormalWorldSpace);
   vec3 specular;
   if (correctSpeclarReflection) {
		vec3 reflectionDirection = reflect(-lightDirection, p_vertexNormalWorldSpace);
		specular = phongSpecularReflection(specularIntensity, specularFactor, lightColor, hardness, viewDirection, reflectionDirection);
   } else {
		vec3 halfNormal = normalize(lightDirection + viewDirection);
		specular = blinnSpecularReflection(specularIntensity, specularFactor, lightColor, hardness, p_vertexNormalWorldSpace, halfNormal);
   }
   vec3 finalColor = (ambient + diffuse + specular) * (p_color.rgb * baseColor);
   fragColor = vec4(finalColor.rgb, 1.0f);
}