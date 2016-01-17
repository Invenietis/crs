export class Action{
    
    name: string;
    properties: any;
    
    constructor(name: string, properties: {}){
        this.name = name;
        this.properties = properties;
    }
}