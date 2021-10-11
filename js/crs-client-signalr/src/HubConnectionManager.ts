import { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";

export class HubConnectionManager {
    private readonly hubConnection: HubConnection;
    private readonly onClosedCallbacks: Function[] = [];
    private connectionCount: number = 0;
    private retryCount: number = 0;
    private connectingPromise?: Promise<HubConnection>;

    public constructor(private readonly url: string, logLevel: LogLevel) {
        this.hubConnection = new HubConnectionBuilder()
            // .configureLogging(LogLevel.Trace)
            .configureLogging(logLevel)
            .withUrl(url)
            .build();
        this.hubConnection.onclose((e) => this.onHubConnectionClosed(e));
    }

    public on(methodName: string, newMethod: (...args: any[]) => any) {
        this.hubConnection.on(methodName, newMethod);
    }

    public off(methodName: string) {
        this.hubConnection.off(methodName);
    }

    public connect(): Promise<HubConnection> {
        ++this.connectionCount;
        if (this.connectionCount === 1) {
            this.ensureConnecting();
        }
        return this.getHubConnection();
    }

    public disconnect(): Promise<void> {
        --this.connectionCount;
        if (this.connectionCount <= 0) {
            return this.doDisconnect();
        }
        return Promise.resolve();
    }

    public getHubConnection(): Promise<HubConnection> {
        if (this.connectingPromise) {
            return this.connectingPromise
        }
        return Promise.resolve(this.hubConnection);
    }

    private static getBackoffTimeMs(retryCount: number) {
        const base = 1000; // 1sec
        const max = 15000; // 15 sec;
        const t = base * retryCount;
        if (t > max) {
            return max;
        }
        return t;
    }

    private ensureConnecting(reconnecting?: boolean): Promise<HubConnection> {
        if (this.connectingPromise === undefined) {
            this.connectingPromise = this.doConnect(reconnecting);
            this.connectingPromise.then(() => {
                this.connectingPromise = undefined;
            }).catch(() => {
                this.connectingPromise = undefined;
            });
        }
        return this.connectingPromise;
    }

    private async doConnect(reconnecting?: boolean): Promise<HubConnection> {
        ++this.retryCount;
        try {
            console.debug(`CRS-SignalR: Connecting to SignalR Hub: ${this.url}`);
            await this.hubConnection.start();
            console.debug(`CRS-SignalR: Connected to SignalR Hub: ${this.url}`);
            this.retryCount = 0;
        } catch (err) {
            console.debug(`CRS-SignalR: Connection failed. Pending retry.`);
            // Wait and retry
            await new Promise(resolve => setTimeout(resolve, HubConnectionManager.getBackoffTimeMs(this.retryCount)));
            if (this.connectionCount > 0) {
                return this.doConnect(true);
            }
        }
        return this.hubConnection;
    }

    private doDisconnect(): Promise<void> {
        console.debug(`CRS-SignalR: Disconnecting`);
        return this.hubConnection.stop();
    }

    private onHubConnectionClosed(err?: Error) {
        for(let i = 0; i < this.onClosedCallbacks.length; i++) {
            try {
                this.onClosedCallbacks[i]();
            } catch(e) {
                console.error(e);
            }
        }
        if (this.connectionCount > 0) {
            console.debug(`CRS-SignalR: Reconnecting`);
            // Reconnect
            this.ensureConnecting(true);
        }
    }

    public onClosed(callback: Function): Function {
        this.onClosedCallbacks.push(callback);
        return () => {
            const idx = this.onClosedCallbacks.indexOf(callback);
            if (idx >= 0) {
                this.onClosedCallbacks.splice(idx, 1);
            }
        }
    }
}
