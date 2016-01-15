export class Command{
    
    name: string;
    properties: any;
    
    constructor(name: string, properties: {}){
        this.name = name;
        properties = this.properties;
    }
}